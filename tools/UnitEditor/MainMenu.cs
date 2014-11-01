using System;
using Gtk;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Data;

namespace UnitEditor
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class MainMenu : Gtk.Bin
	{
		public Gtk.UIManager uim = null;
		public Gtk.MenuBar instance;

		//------------------------------------------------------------------------------
		private XDocument UI = new XDocument (
			new XElement ("ui",
				new XElement ("menubar",
					new XElement ("menu", new XAttribute ("action", "FileMenu"),
						new XElement ("menuitem", new XAttribute ("action", "New")),
						new XElement ("menuitem", new XAttribute ("action", "Open")),
						new XElement ("separator"),
						new XElement ("menuitem", new XAttribute ("action", "Save")),
						new XElement ("menuitem", new XAttribute ("action", "SaveAs")),
						new XElement ("menuitem", new XAttribute ("action", "SaveAll")),
						new XElement ("separator"),
						new XElement ("menuitem", new XAttribute ("action", "Close")),
						new XElement ("menuitem", new XAttribute ("action", "CloseAll")),
						new XElement ("menuitem", new XAttribute ("action", "Quit"))
					),
					new XElement ("menu", new XAttribute ("action", "EditMenu"),
						new XElement ("menuitem", new XAttribute ("action", "Undo")),
						new XElement ("menuitem", new XAttribute ("action", "Redo")),
						new XElement ("separator"),
						new XElement ("menuitem", new XAttribute ("action", "Cut")),
						new XElement ("menuitem", new XAttribute ("action", "Copy")),
						new XElement ("menuitem", new XAttribute ("action", "Paste")),
						new XElement ("menuitem", new XAttribute ("action", "Delete")),
						new XElement ("separator"),
						new XElement ("menuitem", new XAttribute ("action", "SelectAll"))
					),
					new XElement ("menu", new XAttribute ("action", "ViewMenu"),
						new XElement ("menuitem", new XAttribute ("action", "ZoomIn")),
						new XElement ("menuitem", new XAttribute ("action", "ZoomOut")),
						new XElement ("separator"),
						new XElement ("menuitem", new XAttribute ("action", "Fullscreen"))
					),
					new XElement ("menu", new XAttribute ("action", "SearchMenu"),
						new XElement ("menuitem", new XAttribute ("action", "Find")),
						new XElement ("menuitem", new XAttribute ("action", "Replace")),
						new XElement ("separator"),
						new XElement ("menuitem", new XAttribute ("action", "GoToLine"))
					)
				)
			)
		);

		//------------------------------------------------------------------------------
		private Dictionary<string, string> keys = new Dictionary<string, string> () {
			{"New", "<Control>n"},
			{"Open", "<Control>o"},
			{"Save", "<Control>s"},
			{"SaveAs", "<Control><Alt>s"},
			{"SaveAll", "<Control><Shift>s"},
			{"Close", "<Control>w"},
			{"CloseAll", "<Control><Shift>w"},
			{"Quit", "<Alt>F4"},

			{"Undo", "<Control>z"},
			{"Redo", "<Control>y"},
			{"Cut", "<Control>x"},
			{"Copy", "<Control>c"},
			{"Paste", "<Control>v"},
			{"SelectAll", "<Control>a"},

			{"ZoomIn", "<Control>KP_Add"},
			{"ZoomOut", "<Control>KP_Subtract"},
			{"Fullscreen", "F11"},

			{"Find", "<Control>f"},
			{"Replace", "<Control>h"},
			{"GoToLine", "<Control>g"}
		};

		//------------------------------------------------------------------------------
		public Gtk.ActionEntry[] getActionEntries ()
		{
			return new Gtk.ActionEntry[] {
				new Gtk.ActionEntry ("FileMenu", null, "_File", null, null, null),
				new Gtk.ActionEntry ("New", null, "New", (keys.ContainsKey ("New")) ? keys ["New"] : null, null, null),
				new Gtk.ActionEntry ("Open", null, "Open...", (keys.ContainsKey ("Open")) ? keys ["Open"] : null, null, (EventHandler)open_cb),
				new Gtk.ActionEntry ("Save", null, "Save", (keys.ContainsKey ("Save")) ? keys ["Save"] : null, null, null),
				new Gtk.ActionEntry ("SaveAs", null, "Save As...", (keys.ContainsKey ("SaveAs")) ? keys ["SaveAs"] : null, null, null),
				new Gtk.ActionEntry ("SaveAll", null, "Save All", (keys.ContainsKey ("SaveAll")) ? keys ["SaveAll"] : null, null, null),
				new Gtk.ActionEntry ("Close", null, "Close", (keys.ContainsKey ("Close")) ? keys ["Close"] : null, null, null),
				new Gtk.ActionEntry ("CloseAll", null, "Close All", (keys.ContainsKey ("CloseAll")) ? keys ["CloseAll"] : null, null, null),
				new Gtk.ActionEntry ("Quit", null, "Quit", (keys.ContainsKey ("Quit")) ? keys ["Quit"] : null, null, (EventHandler)quit_cb),

				new Gtk.ActionEntry ("EditMenu", null, "_Edit", null, null, null),
				new Gtk.ActionEntry ("Undo", null, "Undo", (keys.ContainsKey ("Undo")) ? keys ["Undo"] : null, null, null),
				new Gtk.ActionEntry ("Redo", null, "Redo", (keys.ContainsKey ("Redo")) ? keys ["Redo"] : null, null, null),
				new Gtk.ActionEntry ("Cut", null, "Cut", (keys.ContainsKey ("Cut")) ? keys ["Cut"] : null, null, null),
				new Gtk.ActionEntry ("Copy", null, "Copy", (keys.ContainsKey ("Copy")) ? keys ["Copy"] : null, null, null),
				new Gtk.ActionEntry ("Paste", null, "Paste", (keys.ContainsKey ("Paste")) ? keys ["Paste"] : null, null, null),
				new Gtk.ActionEntry ("Delete", null, "Delete", (keys.ContainsKey ("Delete")) ? keys ["Delete"] : null, null, null),
				new Gtk.ActionEntry ("SelectAll", null, "Select All", (keys.ContainsKey ("SelectAll")) ? keys ["SelectAll"] : null, null, null),

				new Gtk.ActionEntry ("ViewMenu", null, "_View", null, null, null),
				new Gtk.ActionEntry ("ZoomIn", null, "Zoom In", (keys.ContainsKey ("ZoomIn")) ? keys ["ZoomIn"] : null, null, null),
				new Gtk.ActionEntry ("ZoomOut", null, "Zoom Out", (keys.ContainsKey ("ZoomOut")) ? keys ["ZoomOut"] : null, null, null),
				new Gtk.ActionEntry ("Fullscreen", null, "Fullscreen", (keys.ContainsKey ("Fullscreen")) ? keys ["Fullscreen"] : null, null, null),

				new Gtk.ActionEntry ("SearchMenu", null, "_Search", null, null, null),
				new Gtk.ActionEntry ("Find", null, "Find...", (keys.ContainsKey ("Find")) ? keys ["Find"] : null, null, null),
				new Gtk.ActionEntry ("Replace", null, "Find & Replace...", (keys.ContainsKey ("Replace")) ? keys ["Replace"] : null, null, null),
				new Gtk.ActionEntry ("GoToLine", null, "Go To Line...", (keys.ContainsKey ("GoToLine")) ? keys ["GoToLine"] : null, null, null)
			};
		}

		//------------------------------------------------------------------------------
		public MainMenu ()
		{
			uim = new Gtk.UIManager ();

			Gtk.ActionGroup actions = new Gtk.ActionGroup ("MenuBarActions" + Guid.NewGuid ());

			actions.Add (getActionEntries ());
			uim.InsertActionGroup (actions, 0);
	
			// Put the XML definition of the controls (widgets) into the UIManager's buffer -and- create controls (widgets).
			uim.AddUiFromString (UI.ToString ());

			instance = (Gtk.MenuBar)uim.GetWidget ("/menubar");
			instance.ShowAll ();
		}

		//------------------------------------------------------------------------------
		static void new_cb(object sender, DeleteEventArgs args) {}

		//------------------------------------------------------------------------------
		static void open_cb(object sender, System.EventArgs args)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog("Open file", null, FileChooserAction.Open);
			fc.AddButton(Stock.Cancel, ResponseType.Cancel);
			fc.AddButton(Stock.Open, ResponseType.Ok);
			fc.Filter = new FileFilter();
			fc.Filter.AddPattern("*.unit");
			fc.Filter.AddPattern("*.material");
			fc.Filter.AddPattern("*.physics");

			Console.Write (args.ToString ());
			if (fc.Run() == (int)ResponseType.Ok)
			{
				UnitEditor.MainClass.g_win.open_unit (fc.Filename);
			}
			fc.Destroy();
		}

		//------------------------------------------------------------------------------
		static void save_cb(object sender, DeleteEventArgs args) {}
		static void saveas_cb(object sender, DeleteEventArgs args) {}
		static void saveall_cb(object sender, DeleteEventArgs args) {}
		static void close_cb(object sender, DeleteEventArgs args) {}
		static void closeall_cb(object sender, DeleteEventArgs args) {}
	
		static void quit_cb (object o, EventArgs args)
		{
			Application.Quit ();
		}
	}
}

