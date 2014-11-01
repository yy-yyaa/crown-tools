﻿using System;
using System.Collections.Generic;

namespace crown_tests.GtkExt
{
	public class TreeViewTemplate: ITemplate
	{
		private List<TreeViewRowTemplate> RowTemplates = new List<TreeViewRowTemplate>();
		private List<Tuple<String, Gtk.CellRenderer>> Columns = new List<Tuple<String, Gtk.CellRenderer>>();

		public TreeViewTemplate()
		{
		}

		public TreeViewTemplate AddColumn(String Title, Gtk.CellRenderer renderer)
		{
			Columns.Add(Tuple.Create(Title, renderer));
			return this;
		}

		public TreeViewTemplate AddRowTemplate(TreeViewRowTemplate rowTemplate)
		{
			RowTemplates.Add(rowTemplate);
			return this;
		}

		public void Apply(Gtk.Widget widget) 
		{
			Gtk.TreeView treeView = widget as Gtk.TreeView;
			if (treeView == null) {
				Console.WriteLine("TreeViewTemplate.Apply: Invalid widget type for this template");
				return;
			}

			foreach (var col in Columns) {
				treeView.AppendColumn(col.Item1, col.Item2, ValuePropertyDataFunc);
			}
		}

		private static void ValuePropertyDataFunc(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			var treeView = (Gtk.TreeView)column.TreeView;
			var info = Templating.GetTemplate(treeView) as TreeViewTemplate;

			var textCell = (cell as Gtk.CellRendererText);
			textCell.Text = string.Empty;
			var value = model.GetValue(iter, 0);
			if (value == null)
				return;

			foreach (var rowTemplate in info.RowTemplates) {
				if (value.GetType() == rowTemplate.TargetType) {
					//Here we have a value, which is the source for Binding, and a BindingInfo that is given by rowTemplate.ColumnBindings[column.Title] .
					//The instance of the BindingInfo is shared among all values (rows), since it was defined once in the rowTemplate.

					BindingInfo bindingInfo = null;
					if (!rowTemplate.ColumnBindings.TryGetValue(column.Title, out bindingInfo))
						return;

					//The actual binding, on the other hand, is specific to the current (row,column) pair.
					Binding binding = BindingEngine.GetOrCreateBinding(treeView, value, new TreeViewIterBindingTarget(treeView, iter, column), bindingInfo);
					var propValue = binding.GetSourceValue();
					textCell.Text = propValue == null ? String.Empty : propValue.ToString();
					return;
				}
			}
		}
	}
}

