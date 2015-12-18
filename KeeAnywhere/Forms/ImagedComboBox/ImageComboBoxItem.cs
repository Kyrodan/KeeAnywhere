using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace KeeAnywhere.Forms.ImagedComboBox
{
	[ToolboxItem(false)]
	[DesignTimeVisible(false)]
	[Serializable()]
	[TypeConverter(typeof(ImageComboItemConverter))]

	public sealed class ImageComboBoxItem : ISerializable
	{
		private string imageIndexAsString = "(none)";
		private int indexOfImage = -1;
		private string ItemText = string.Empty ;
		private Font font = null;
		private object item = null;
		private int indent = 0;

		public ImageComboBoxItem()
		{
		
		}
		public ImageComboBoxItem(string text,int indentLevel)
		{
			this.ItemText = text;
		}
		public ImageComboBoxItem(int imageIndex, string text,int indentLevel)
		{
			this.ImageIndex = imageIndex;
			this.ItemText = text;
			
		}
		public ImageComboBoxItem(int imageIndex, string text,Font font,int indentLevel)
		{
			this.ImageIndex = imageIndex;
			this.ItemText = text;
			this.font = font;
		}
		
		public ImageComboBoxItem(object item)
		{
			this.Item = item;
			this.Text = item.ToString ();
		}
		

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Editor(typeof(DropDownImages),typeof(System.Drawing.Design.UITypeEditor))]
		public string Image
		{
			get
			{
				return imageIndexAsString;		
			}
			set
			{
				if(value != null)
				{
					imageIndexAsString = value.ToString ();
					if (imageIndexAsString.Equals ("(none)")== true)
						ImageIndex = -1;
					else
						ImageIndex = System.Convert .ToInt32 (imageIndexAsString.ToString ());
				}
			
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public int ImageIndex
		{
			get
			{
				return indexOfImage;
			}
			set
			{
				indexOfImage = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public object Item
		{
			get
			{
				return item;
			}
			set
			{
				item = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
			}
		}

		[Localizable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Text
		{
			get
			{
				return ItemText;

			}
			set
			{
				ItemText = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public int IndentLevel
		{
			get
			{
				return indent;
			}
			set
			{
				indent = value;
				if(indent < 0 )
					throw new System.Exception ("Please enter a value greater than 0 and less than 5 for indentation."+Environment.NewLine +"Supported indentation levels are from 0-5");
				if(indent > 4 )
					throw new System.Exception ("Please enter a value greater than 0 and less than 5 for indentation."+Environment.NewLine +"Supported indentation levels are from 0-5");
			}
		}
		#region ISerializable implemented members...

		public ImageComboBoxItem(SerializationInfo info, StreamingContext context) 
		{
			this.ItemText = (string)info.GetValue("Text", typeof(string));
			this.indexOfImage = (int)info.GetValue("ImageIndex", typeof(int));
			this.font = (Font)info.GetValue ("Font",typeof(Font));
			this.Item = (object)info.GetValue ("Item",typeof(object));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Text", this.ItemText);
			info.AddValue("ImageIndex", this.indexOfImage);
			info.AddValue ("Font",this.font );
			info.AddValue ("Item",this.item );
		}

		#endregion

		public override string ToString()
		{
			if(this.Item != null)
				return this.Item .ToString ();
			return Text;
		}
		public object Clone() 
		{
			if(this.Item != null)
				return new ImageComboBoxItem (item);
			return new ImageComboBoxItem(ImageIndex,ItemText,font,indent);
		}


	}
}
