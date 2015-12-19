using System;
using System.Collections;
using System.ComponentModel;

namespace KeeAnywhere.Forms.ImagedComboBox
{
	/// <summary>
	/// The ImageComboBoxItems are held in this ImageComboBoxItemCollection.
	/// </summary>
	public sealed class ImageComboBoxItemCollection : IList, ICollection, IEnumerable
	{
		private KeeAnywhere.Forms.ImagedComboBox.ImageComboBox owner = null;

		public ImageComboBoxItemCollection(KeeAnywhere.Forms.ImagedComboBox.ImageComboBox owner)
		{
			this.owner = owner;
		}

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index) 
		{
			for (IEnumerator e = this.GetEnumerator(); e.MoveNext();)
				array.SetValue(e.Current, index++);
		}
		
		bool ICollection.IsSynchronized 
		{
			get { return false; }
		}

		object ICollection.SyncRoot 
		{
			get { return this; }
		}

		#endregion

		#region IList Members

		object IList.this[int index] 
		{
			get { return this[index]; }
			set { this[index] = (ImageComboBoxItem)value; }
		}

		bool IList.Contains(object item)
		{
			throw new NotSupportedException();
		}

		int IList.Add(object item)
		{
			ImageComboBoxItem comboItem = (ImageComboBoxItem)item;
			return this.Add(comboItem);
		}

		bool IList.IsFixedSize 
		{
			get { return false; }
		}

		int IList.IndexOf(object item)
		{
			throw new NotSupportedException();
		}

		void IList.Insert(int index, object item)
		{
			ImageComboBoxItem comboItem = (ImageComboBoxItem)item;
			this.Insert(index, comboItem);
		}

		void IList.Remove(object item)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		#endregion

		#region ImageComboBox functions
		[Browsable(false)]
		public ImageComboBoxItem this[int index]
		{
			get{return (ImageComboBoxItem)owner.ComboBoxGetElement (index);}
			set{owner.ComboBoxSetElement (index,value);}
		}
		public int Count 
		{
			get { return owner.ComboBoxGetItemCount(); }
		}

		public bool IsReadOnly 
		{
			get { return false; }
		}
		
		public IEnumerator GetEnumerator() 
		{
			return owner.ComboBoxGetEnumerator(); 
		}

		public bool Contains(object item)
		{
			throw new NotSupportedException();
		}

		public int IndexOf(object item)
		{
			throw new NotSupportedException();
		}

		public void Remove(ImageComboBoxItem item)
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, ImageComboBoxItem item)
		{
			owner.ComboBoxInsertItem(index, item);
		}

		public int Add(ImageComboBoxItem item)
		{
			return owner.ComboBoxInsertItem(this.Count, item);
		}

		public void AddRange(ImageComboBoxItem[] items)
		{
			for(IEnumerator e = items.GetEnumerator(); e.MoveNext();)
				owner.ComboBoxInsertItem(this.Count, (ImageComboBoxItem)e.Current);
		}

		public void Clear()
		{
			owner.ComboBoxClear();
		}

		public void RemoveAt(int index)
		{
			owner.ComboBoxRemoveItemAt(index);
		}

		#endregion
	}
		
}
