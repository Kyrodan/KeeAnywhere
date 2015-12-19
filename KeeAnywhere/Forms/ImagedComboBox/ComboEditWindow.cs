using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeeAnywhere.Forms.ImagedComboBox
{
	#region NativeWindow for EditBox
	/// <summary>
	/// The ComboEditWindow is a helper class, to get access to the windows message 
	/// stream directed towards the Edit portion of the ComboBox. This class gets 
	/// assigned the handle of the TextBox of the ComboBox.
	/// </summary>
	public sealed class ComboEditWindow : System.Windows.Forms.NativeWindow 
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		
		[DllImport("user32", CharSet=CharSet.Auto)]
		private extern static IntPtr FindWindowEx(
			IntPtr hwndParent, 
			IntPtr hwndChildAfter,
			[MarshalAs(UnmanagedType.LPTStr)]
			string lpszClass,
			[MarshalAs(UnmanagedType.LPTStr)]
			string lpszWindow);

		[DllImport("user32")] 
		private static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref ComboBoxInfo info);

		[StructLayout(LayoutKind.Sequential)]
		private struct ComboBoxInfo 
		{
			public int cbSize;
			public RECT rcItem;
			public RECT rcButton;
			public IntPtr stateButton;
			public IntPtr hwndCombo;
			public IntPtr hwndEdit;
			public IntPtr hwndList; 
		}
		[DllImport("user32", CharSet=CharSet.Auto)]
		private extern static int SendMessage(
			IntPtr hwnd, 
			int wMsg,
			int wParam, 
			int lParam);

		private const int EC_LEFTMARGIN = 0x1;
		private const int EC_RIGHTMARGIN = 0x2;
		private const int WM_PAINT = 0xF;
		private const int WM_SETCURSOR = 0x20;
		private const int WM_LBUTTONDOWN = 0x201;
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int WM_CHAR = 0x102;
		private const int WM_GETTEXTLENGTH = 0xe;
		private const int WM_GETTEXT = 0xd;
		private const int EM_SETMARGINS = 0xD3;

		private Graphics gfx;
		private Image icon = null;
		private int Margin = 0;
		private int index = -1;
		private ComboBoxInfo cbxinfo = new ComboBoxInfo() ;
		private ImageComboBox Owner = null;

		public ComboEditWindow()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		public Image CurrentIcon
		{
			get
			{
				return icon;
			}
			set
			{
				icon = value;
			}
		}
		
		
		/// <summary>
		/// The native window's original handle is released 
		/// and the handle of the TextBox is assigned to it.
		/// </summary>
		public void AssignTextBoxHandle(ImageComboBox owner )
		{
			
			Owner = owner;
			cbxinfo.cbSize =  Marshal.SizeOf(cbxinfo);
			GetComboBoxInfo(Owner.Handle, ref cbxinfo);

			if (!this.Handle.Equals(IntPtr.Zero))
			{
				this.ReleaseHandle();
			}
			this.AssignHandle(cbxinfo.hwndEdit);
		}
		
		/// <summary>
		/// The image to be displayed in the Edit Portion.
		/// </summary>
		public void SetImageToDraw()
		{
			if(Owner == null)
				return;
			
			index = Owner.SelectedIndex;
			
			if((index > -1))
			{
				int imageindex = ((ImageComboBoxItem)Owner.Items [index ]).ImageIndex;
				if(imageindex != -1)
				{
					CurrentIcon = new Bitmap(Owner.ImageList.Images [imageindex],Owner.ItemHeight,Owner.ItemHeight);
					if(Owner.RightToLeft == RightToLeft.Yes)
						CurrentIcon.RotateFlip(RotateFlipType.RotateNoneFlipX);

					Margin = CurrentIcon.Width+2; // required width to display the image correctly
				}
				else
				{
					CurrentIcon = null;
					Margin = 0;
				}
				
			}
		}
		public void SetMargin()
		{
			int leftMargin = 0;
			int rightMargin = 0;
			
			if(Owner == null)
				return;
			
			// If combobox is RightToLeft Aligned, margin is set to the right of the combobox.
			if(Owner.RightToLeft == RightToLeft.Yes)
			{
				// To set the right margin, the lparam's hiword is taken as the margin.
				// hence,to shift the margin to hiword, we multiply the it with 65536.

				rightMargin = Margin;
				SendMessage(this.Handle, EM_SETMARGINS, EC_RIGHTMARGIN, rightMargin * 65536);
				SendMessage(this.Handle, EM_SETMARGINS, EC_LEFTMARGIN, leftMargin);
			}// If combobox is LeftToRight Aligned, margin is set to the left of the combobox.
			else if(Owner.RightToLeft == RightToLeft.No)
			{
				// To set the left margin, the lparam's loword is taken as the margin.
				leftMargin = Margin;
				SendMessage(this.Handle, EM_SETMARGINS, EC_LEFTMARGIN, Margin);	
				SendMessage(this.Handle, EM_SETMARGINS, EC_RIGHTMARGIN, rightMargin * 65536);
			}
			
		}
		
		/// <summary>
		/// Whenever the textbox is repainted, we have to draw the image.
		/// </summary>
		public void DrawImage()
		{
			if((CurrentIcon!=null))
			{	
				// Gets a GDI drawing surface from the textbox.
				gfx = Graphics.FromHwnd (this.Handle);
				bool rightToLeft = false;
				if(Owner.RightToLeft == RightToLeft.Inherit)
				{
					if(Owner.Parent.RightToLeft == RightToLeft.Yes)
						rightToLeft =  true;

				}
				if(Owner.RightToLeft == RightToLeft.Yes || rightToLeft)
				{
					gfx.DrawImage(CurrentIcon,gfx.VisibleClipBounds.Width - CurrentIcon.Width,0);
				}
				else if(Owner.RightToLeft == RightToLeft.No || rightToLeft)
					gfx.DrawImage(CurrentIcon,0,0);
				
				gfx.Flush();
				gfx.Dispose();
			}
		}

		// Override the WndProc method so that we can redraw the TextBox when the textbox is repainted.
		protected override void WndProc(ref Message m)
		{
			
			switch (m.Msg)
			{
				case WM_PAINT:
					base.WndProc(ref m);
					DrawImage();
					break;
				
				case WM_LBUTTONDOWN:
					base.WndProc(ref m);
					DrawImage();
					break;
				case WM_KEYDOWN:
					base.WndProc(ref m);
					DrawImage();
					break;
				case WM_KEYUP:
					base.WndProc(ref m);
					DrawImage();
					break;
				case WM_CHAR:
					base.WndProc(ref m);
					DrawImage();
					break;
				case WM_GETTEXTLENGTH:
					base.WndProc(ref m);
					DrawImage();
					break;
				case WM_GETTEXT:
					base.WndProc(ref m);
					DrawImage();
					break;
				default:
					base.WndProc(ref m);
					break;
			}
		}
	}

	#endregion NativeWindow for EditBox
}
