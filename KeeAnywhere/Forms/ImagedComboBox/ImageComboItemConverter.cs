using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace KeeAnywhere.Forms.ImagedComboBox
{
	/// <summary>
	/// The Combobox items are not strings, instead they are objects of ImageComboBoxItems. So a type converter is needed 
	/// 
	/// </summary>
	public sealed class ImageComboItemConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if(destinationType == typeof(System.ComponentModel.Design.Serialization .InstanceDescriptor))
				return true;
			else
				return base.CanConvertTo (context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType ==  typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
			{
				Type valueType = value.GetType();
				ConstructorInfo ci = valueType.GetConstructor(System.Type.EmptyTypes);
				ImageComboBoxItem item = (ImageComboBoxItem)value;
				return new InstanceDescriptor(ci,null,false);
			}
			else
				return base.ConvertTo (context, culture, value, destinationType);
		}

		
	}
}
