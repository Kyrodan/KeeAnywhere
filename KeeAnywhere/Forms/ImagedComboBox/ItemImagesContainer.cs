using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KeeAnywhere.Forms.ImagedComboBox
{
	/// <summary>
	/// Tis class is associted with the Image Prperty of ImageComboBoxItem class.
	/// It displays the images from the imagelist, in the Items collection Editor.
	/// </summary>
	public class ItemImagesContainer : System.Windows.Forms.Form
	{

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private ImageList imageList = null;
		private System.Windows.Forms.ListBox imageListBox;
		private Image noneIcon = null;
		private string selectedItem = string.Empty;
		public delegate void AfterSelectEventHandler(bool canCloseNow);
		public event AfterSelectEventHandler AfterSelectEvent;
		
		public ItemImagesContainer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			imageListBox.Size = new Size (this.Size.Width-2,this.Size.Height-2);
			this.TopLevel = false;
			// We create an empty bitmap, corresponding to the item none. ie when the item doesn't have any image to be displayed with it.
			int dimension=16;
			Bitmap emptyBMP = new Bitmap(dimension,dimension);
			Graphics graphics = Graphics.FromImage((Image)emptyBMP); 
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle imgRect = new Rectangle(0,0,emptyBMP.Width-1,emptyBMP.Height-1);
			graphics.DrawRectangle (new Pen (new SolidBrush (Color.Black)),imgRect);
			graphics.FillRectangle(new SolidBrush (Color.White),imgRect);
			noneIcon = Image.FromHbitmap(emptyBMP.GetHbitmap()); 
			graphics.Dispose();
			emptyBMP.Dispose();
			this.imageListBox .DrawMode = DrawMode.OwnerDrawFixed;
			this.imageListBox.ItemHeight = 25;
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.imageListBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// imageListBox
			// 
			this.imageListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.imageListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.imageListBox.IntegralHeight = false;
			this.imageListBox.Location = new System.Drawing.Point(0, 0);
			this.imageListBox.MultiColumn = true;
			this.imageListBox.Name = "imageListBox";
			this.imageListBox.Size = new System.Drawing.Size(180, 150);
			this.imageListBox.TabIndex = 0;
			this.imageListBox.SizeChanged += new System.EventHandler(this.imageListBox_SizeChanged);
			this.imageListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.imageListBox_DrawItem);
			this.imageListBox.SelectedIndexChanged += new System.EventHandler(this.imageListBox_SelectedIndexChanged);
			// 
			// ItemImagesContainer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(178, 88);
			this.ControlBox = false;
			this.Controls.Add(this.imageListBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ItemImagesContainer";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Load += new System.EventHandler(this.ComboItemImageContainer_Load);
			this.ResumeLayout(false);

		}
		#endregion
		/// <summary>
		/// The Images from the imageList are added to the ListBox.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				imageList = value;
				if(imageList == null || imageList.Images.Count == 0)
				{
					this.imageListBox.Items .Clear();
					this.imageListBox.Items.Add ("(none)");
				}
				if(imageList != null && imageList.Images.Count > 0)
				{
					this.imageListBox.Items .Clear();
					this.imageListBox.ItemHeight = imageList.ImageSize.Height + 10;
					for(int i=0; i<imageList.Images .Count; i++)
						this.imageListBox.Items.Add(i);

					this.imageListBox.Items.Add ("(none)");
				}
			}
		
		}
		public Image ListBoxIcon
		{
			get
			{
				return this.noneIcon;
			}
		}
		public string SelectedItem
		{
			get
			{
				return selectedItem;
				
			}
			set
			{
				selectedItem = value;
			}
		}
		private void ComboItemImageContainer_Load(object sender, System.EventArgs e)
		{
			imageListBox.SelectedIndex = -1;
		}

		/// <summary>
		/// Because the ListBox is ownerdrawn we have to draw each item manually.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void imageListBox_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			if (e.Index >= 0 && this.imageListBox.Items.Count > 0 && e.Index < this.imageListBox.Items.Count )
			{
				e.DrawBackground();
				e.DrawFocusRectangle ();
				
				if(this.ImageList == null || this.ImageList .Images .Count == 0 || e.Index == this.imageListBox.Items.Count - 1)
				{
					e.Graphics.DrawImage (this.noneIcon ,new Rectangle (e.Bounds.X+2,e.Bounds.Y+5,this.noneIcon.Width ,this.noneIcon.Height));
					e.Graphics.DrawString(this.imageListBox.Items [e.Index ].ToString (),e.Font ,new SolidBrush(e.ForeColor),(float)(e.Bounds .X+ 16 + 3),(float)(e.Bounds.Y+5));
				}
				else
				{
					if(e.Index < this.imageListBox.Items.Count - 1)
						e.Graphics.DrawImage (this.ImageList.Images [e.Index ],new Rectangle (e.Bounds.X+2,e.Bounds.Y+5,this.ImageList.ImageSize.Width ,this.ImageList.ImageSize.Height));
					else
						e.Graphics.DrawImage (this.noneIcon ,new Rectangle (e.Bounds.X+2,e.Bounds.Y+5,this.noneIcon.Width ,this.noneIcon.Height));
					e.Graphics.DrawString(this.imageListBox.Items [e.Index ].ToString (),e.Font ,new SolidBrush(e.ForeColor),(float)(e.Bounds.X+this.ImageList.Images[e.Index].Width + 3),(float)(e.Bounds.Y+5));
				}
			}
		}

		private void imageListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(imageListBox.SelectedItem.ToString().CompareTo("(none)") == 0)
				this.SelectedItem =  "(none)";
			else
				this.SelectedItem = this.imageListBox.SelectedIndex.ToString ();

			OnAfterSelectEvent(true);
		}

		private void imageListBox_SizeChanged(object sender, System.EventArgs e)
		{
			this.Size = new Size (this.imageListBox.Size.Width,this.imageListBox.Size.Height);
		}

		protected virtual void OnAfterSelectEvent(bool canCloseNow)
		{
			if(AfterSelectEvent != null)
				AfterSelectEvent(canCloseNow);
		}
	}
}
