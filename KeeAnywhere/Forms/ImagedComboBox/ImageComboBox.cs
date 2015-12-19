using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeeAnywhere.Forms.ImagedComboBox
{
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(ImageComboBox),"ComboBMP.bmp")]
	//[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
	public class ImageComboBox : System.Windows.Forms.ComboBox 
	{
		#region Variables
		private DrawMode ComboBoxDrawMode = DrawMode.OwnerDrawFixed ; // set the default drawmode to owner drawselected.
		private ImageComboBoxItemCollection ListItems; // the collection of imagecomboboxitems
		private string ItemText = string.Empty;
		private ImageList imgList = null; // the images to be used in the imagecombobox are stored in an imagelist.
		private ComboEditWindow EditBox = new ComboEditWindow(); // the NativeWindow object, used to access and repaint the TextBox.
		private object[] DrawModes = new object[2];
		private string ComboText = string.Empty ;
		private int indentValue = 0;

		[DllImport("user32", CharSet=CharSet.Auto)]
		private extern static IntPtr FindWindowEx(
			IntPtr hwndParent, 
			IntPtr hwndChildAfter,
			[MarshalAs(UnmanagedType.LPTStr)]
			string lpszClass,
			[MarshalAs(UnmanagedType.LPTStr)]
			string lpszWindow);

		
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		
		#endregion Variables

		public ImageComboBox()
		{
			// supported drawmodes are OwnerDrawFixed and OwnerDrawVariable only.

			this.DrawMode = DrawMode.OwnerDrawFixed ;
			DrawModes[0] = DrawMode.OwnerDrawFixed ;
			DrawModes[1]= DrawMode.OwnerDrawVariable ;
			base.ItemHeight = 15;
			ListItems = new ImageComboBoxItemCollection (this);
		}


		#region Properties
		
		
		/// <summary>
		/// The imagelist holds the images displayed with the items in the combobox.
		/// </summary>
		[Category("Behavior")]
		[Browsable(true)]
		[Description("The ImageList control from which the images to be displayed with the items are taken.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ImageList ImageList
		{
			get
			{
				return imgList;
			}
			set
			{
				imgList = value;
				// prepare the dropdown Images List from which user can choose an image for corresponding item
				DropDownImages.imageList = imgList; 
			}

		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new int ItemHeight
		{
			get
			{
				return base.ItemHeight; 
			}
			set
			{
				base.ItemHeight = value;
			}
		}

		
		/// <summary>
		/// A new drawmode property is added so that only ownerdrawn and ownerdrawvariable are displayed.
		/// </summary>
 
		[Browsable(true)]
		[Editor(typeof(DropDownDrawModes),typeof(System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new DrawMode DrawMode
		{
			get
			{
				DropDownDrawModes.List =DrawModes;
				return ComboBoxDrawMode;
			}
			set
			{
				DropDownDrawModes.List =DrawModes;
				if(value == DrawMode.OwnerDrawFixed )
				{
					ComboBoxDrawMode = DrawMode.OwnerDrawFixed ;
					base.DrawMode = DrawMode.OwnerDrawFixed ;
				}
				else if(value == DrawMode.OwnerDrawVariable )
				{
					ComboBoxDrawMode = DrawMode.OwnerDrawVariable ;
					base.DrawMode = DrawMode.OwnerDrawVariable  ;
				}
				else
					throw new System .Exception ("The ImageComboBox does not support the "+value.ToString()+" mode.");
			}
		}

		/// <summary>
		/// A new Items Property is introduced, so that the items of type ImageComboBoxItem could be 
		/// added/removed/edited. An ImageComboBoxItem can have a corresponding image and font property.
		/// So using the ImageComboBoxItem collection editor, the image and font can be set on the spot.
		/// </summary>
		[Category("Behavior")]  
		[Description("The collection of items in the ImageComboBox.")]
		[Localizable(true)]
		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		//[EditorAttribute(typeof(CollectionEditor),typeof(UITypeEditor))]
		[TypeConverter(typeof(ImageComboBoxItemCollection))]
		public new ImageComboBoxItemCollection Items
		{
			get
			{
				this.ImageList = imgList;
				return this.ListItems;
			}
			set
			{
				// when the datasource property is set, it takes precedence over Items Property.
				// This is the default combobox behavior. 
				if(this.DataSource != null)
					throw new System.Exception ("The Items cannot be used concurrently with the DataSource.");
				else
					this.ListItems = (ImageComboBoxItemCollection)value;
			}
			
		}
		/// <summary>
		/// The ImageComboBox supports three levels of indentation.
		/// The Indent property represents the width of indentation of each item in pixels.
		/// For each level the value of Indent is multiplied by the indent level to properly indent the item;
		/// </summary>
		[Category("Behavior")]  
		[Browsable(true)]
		[Description("The Indentation width of an item in pixels.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public int Indent
		{
			get
			{
				return indentValue;
			}
			set
			{
				indentValue = value;
			}
		}
				
		#endregion Properties

		#region Methods to access base class items.

		public void ComboBoxSetElement(int index, object value)
		{
			base.Items[index] = value;
		}
		
		public ImageComboBoxItem ComboBoxGetElement(int index)
		{
			return (ImageComboBoxItem)base.Items[index];
		}

		public IEnumerator ComboBoxGetEnumerator()
		{
			return base.Items.GetEnumerator();
		}

		public int ComboBoxGetItemCount()
		{
			return base.Items.Count;
		}
		
		public bool ComboBoxContains(ImageComboBoxItem item)
		{
			return base.Items.Contains (item);
		}
		
		public int ComboBoxInsertItem(int index, ImageComboBoxItem item)
		{
			item.Text = (item.Text.Length == 0)?item.GetType ().Name+index.ToString ():item.Text;
			base.Items.Insert(index, item);
			return index;
		}
		
		public int ComboBoxAddItem(ImageComboBoxItem item)
		{
			item.Text = (item.Text.Length == 0)?item.GetType ().Name+base.Items.Count.ToString ():item.Text;
			base.Items.Add (item);
		
			return base.Items.Count-1;
		}

		public void ComboBoxRemoveItemAt(int index)
		{
			base.Items.RemoveAt(index);
		}
		public void ComboBoxRemoveItem(ImageComboBoxItem item)
		{
			base.Items.Remove (item);
		}
		public void ComboBoxClear()
		{
			base.Items.Clear();
		}

		#endregion

		#region Overrided Methods
		
		

		

		
		/// <summary>
		/// Once the handle of the ImageComboBox is available, we can release NativeWindow's 
		/// own handle and assign the TextBox'es handle to it.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated (e);
			if((this.DropDownStyle == ComboBoxStyle.DropDown) || (this.DropDownStyle == ComboBoxStyle.Simple))
				EditBox.AssignTextBoxHandle(this);
		}
		/// <summary>
		/// When the RightToLeft property is changed the margin of the TextBox also should be changed accordingly.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged (e);

			if((this.DropDownStyle == ComboBoxStyle.DropDown) || (this.DropDownStyle == ComboBoxStyle.Simple))
			{
				EditBox.SetMargin();
				EditBox.SetImageToDraw(); // the image on the textbox has to be redrawn
			}
		}
		/// <summary>
		/// The TextBox need to be updated when combobox'es selection changes.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged (e);
			object selectedItem = base.Items [this.SelectedIndex];
			
			EditBox.SetImageToDraw(); // the image on the textbox has to be redrawn
			EditBox.SetMargin(); // the margin needs be set according the width of the image.
		}
		
		
		
		/// <summary>
		/// when the imagecombobox drawmode is ownerdraw variable, 
		/// each item's height and width need to be measured, inorder to display them properly.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			base.OnMeasureItem (e);
			if(this.DataSource != null)
			{
				return; // currently ownerdrawvariable support is implemented only for Items Collection.
			}
			if (e.Index >= 0 && this.Items.Count > 0 && e.Index < this.Items.Count)
			{
				Font itemFont =((ImageComboBoxItem) this.Items[e.Index]).Font == null ? this.Font : ((ImageComboBoxItem)this.Items[e.Index]).Font;
				SizeF TextSize  = e.Graphics .MeasureString (((ImageComboBoxItem)this.Items[e.Index ]).Text,itemFont);
				e.ItemHeight = (int)TextSize.Height;
				e.ItemWidth = (int)TextSize.Width;
			}
		}
		/// <summary>
		/// Because the combobox is ownerdrawn we have draw each item along with the associated image.
		/// In the case of datasource the displaymember and imagemember are taken from the datasource.
		/// If datasource is not set the items in the Items collection are drawn.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem (e);
			if (e.Index >= 0 && Items.Count > 0 && e.Index < Items.Count)
			{
				DrawItemHelperForItems(e);
			}
		}

		
		/// <summary>
		/// Helper method for drawing items .
		/// </summary>
		/// <param name="e"></param>
		private void DrawItemHelperForItems(DrawItemEventArgs e)
		{
			e.DrawFocusRectangle ();
			e.DrawBackground ();
			
			ImageComboBoxItem item = (ImageComboBoxItem)this.Items [e.Index ];
			if(item.Font == null)
				item.Font = this.Font;
					
			StringFormat format = new StringFormat ();
			//format.FormatFlags = StringFormatFlags.NoWrap;
			format.FormatFlags = StringFormatFlags.NoClip;
			int indent = (item.IndentLevel*this.Indent);

			if(item.ImageIndex != -1)
			{	
				Image theIcon = this.ImageList.Images [item.ImageIndex ];
				Bitmap ImageToDraw = new Bitmap (theIcon,e.Bounds.Height-1,e.Bounds.Height-1);
				int IconHeight = ImageToDraw.Height;
				int IconWidth = ImageToDraw.Width;
				int offset = 1;
				if(this.RightToLeft == RightToLeft.Yes)
				{
					ImageToDraw.RotateFlip(RotateFlipType.RotateNoneFlipX);
					format.Alignment = StringAlignment.Far;
					RectangleF itemRect = new RectangleF ((float)(e.Bounds.X-offset),(float)(e.Bounds .Y),(float)(e.Bounds.Width-IconWidth-indent-offset) ,(float)(e.Bounds.Height));
					e.Graphics.DrawString (item.Text,item.Font,new SolidBrush (e.ForeColor),itemRect,format);
					Rectangle imageRect = new Rectangle(e.Bounds.X+offset+(e.Bounds.Width-(IconWidth+indent)),e.Bounds.Y,IconWidth,IconHeight);
					e.Graphics.DrawImage(ImageToDraw,imageRect);
				}
				else
				{
					format.Alignment = StringAlignment.Near ;
					RectangleF itemRect = new RectangleF ((float)(e.Bounds.X+IconWidth+indent+offset),(float)(e.Bounds .Y),(float)(e.Bounds .Width-IconWidth-indent-offset),(float)(e.Bounds.Height)) ;
					e.Graphics.DrawString (item.Text,item.Font,new SolidBrush (e.ForeColor),itemRect,format);
					Rectangle imageRect = new Rectangle(e.Bounds.X+offset+indent,e.Bounds.Y,IconWidth,IconHeight);
					e.Graphics.DrawImage(ImageToDraw,imageRect);
				}
			}
			else
			{
				if(this.RightToLeft == RightToLeft.Yes)
				{
					format.Alignment = StringAlignment.Far ;
					e.Graphics.DrawString (item.Text,item.Font,new SolidBrush (e.ForeColor),new RectangleF ((float)e.Bounds .X,(float)e.Bounds .Y,(float)(e.Bounds .Width - indent),(float)e.Bounds.Height),format);
				}
				else
				{
					format.Alignment = StringAlignment.Near ;
					e.Graphics.DrawString (item.Text,item.Font,new SolidBrush (e.ForeColor),new RectangleF ((float)(e.Bounds .X+indent),(float)e.Bounds .Y,(float)e.Bounds .Width ,(float)e.Bounds.Height),format);
				}
			}
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged (e);
			if(this.SelectedIndex > -1)
			{
				object selectedItem = base.Items [this.SelectedIndex];
				
				EditBox.SetImageToDraw();
				EditBox.SetMargin();
			}
			
		}

		#endregion Overrided Methods
		
		
	}

}
