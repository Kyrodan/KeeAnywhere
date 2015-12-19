using System;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace KeeAnywhere.Forms.ImagedComboBox
{
	/// <summary>
	/// The DropDownImages class is associtaed with the Image Property of the ImageComboBoxItem.
	/// The Items of the ImageComboBox now exposes an item collection editor instead of string collection editor.
	/// The item is of type ImageComboBoxItem, which supports Font and an Image specified for the item. The Images 
	/// are stored in an ImageList and are displayed in the collection editor in a dropdown imagelistbox. 
	/// The imageListbox, in turn is an ownerdrawn  Listbox to display images.
	/// </summary>
	public sealed class DropDownImages: System.Drawing.Design.UITypeEditor
	{
		private ItemImagesContainer imagesContainer = new ItemImagesContainer ();
		private IWindowsFormsEditorService edSvc;
		public static ImageList imageList = null;

		public DropDownImages()
		{
			imagesContainer.AfterSelectEvent+=new ItemImagesContainer.AfterSelectEventHandler(imagesContainer_AfterSelectEvent);
		}
	
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{	
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			imagesContainer.ImageList = imageList;
			
				if( edSvc != null )
				{
					edSvc.DropDownControl(imagesContainer);
					if(imagesContainer.SelectedItem != string.Empty)
						return imagesContainer.SelectedItem.ToString();
					
				}
			
			return value;
		}
		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
		
		public override void PaintValue(PaintValueEventArgs e)
		{
			if(e.Value .ToString ().CompareTo ("(none)")==0 )
			{
				e.Graphics.DrawImage(imagesContainer.ListBoxIcon ,e.Bounds);
				string dispayStr =e.Value .ToString ();
				e.Graphics.DrawString (dispayStr,imagesContainer.Font , new SolidBrush (imagesContainer.ForeColor),new Rectangle(e.Bounds.Width+10,0,(int)e.Graphics.MeasureString (dispayStr,imagesContainer.Font).Width+10,e.Bounds .Height)); 
			}
			if(imageList != null && imageList.Images .Count > 0)
			{
				Image img = imageList.Images[System.Convert.ToInt32 (e.Value .ToString ())];
				e.Graphics.DrawImage(img,e.Bounds);
			}
		}

		private void imagesContainer_AfterSelectEvent(bool canCloseNow)
		{
			if(canCloseNow == true)
				edSvc.CloseDropDown();
		}
	}
}
