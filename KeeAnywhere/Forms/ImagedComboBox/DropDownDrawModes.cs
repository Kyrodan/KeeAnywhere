using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace KeeAnywhere.Forms.ImagedComboBox
{
	/// <summary>
	/// This class is associated with the DrawMode Property of the ImageComboBox.
	/// The ImageComboBox needs to support only two DrawModes - OwnerDrawFixed and OwnerDrawVariable.
	/// The DropDownDrawModes class is a UITypeEditor which displays the DrawModes in a dropdown ListBox.
	/// </summary>
	public sealed class DropDownDrawModes : System.Drawing.Design.UITypeEditor
	{
		private ListBox DrawModeList = new ListBox();
		private IWindowsFormsEditorService edSvc;

		public static object[] List;

		public DropDownDrawModes()
		{
			DrawModeList.BorderStyle=BorderStyle.None;
			DrawModeList.Click+=new EventHandler(DrawModeList_Click);
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			DrawModeList.Items.Clear();
			DrawModeList.Items.AddRange(List);
			DrawModeList.Height=DrawModeList.PreferredHeight;
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if( edSvc != null )
			{
				edSvc.DropDownControl( DrawModeList );
				return DrawModeList.SelectedItem;

			}
			return value;
		}

		private void DrawModeList_Click(object sender, EventArgs e)
		{
			edSvc.CloseDropDown();
		}
	}
}
