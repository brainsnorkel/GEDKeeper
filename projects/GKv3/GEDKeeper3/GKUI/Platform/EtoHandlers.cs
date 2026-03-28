#pragma warning disable CA1416

using System.Collections.Generic;
using Eto.Drawing;

#if OS_MSWIN
using Eto.Wpf;
using Eto.Wpf.Drawing;
using Eto.Wpf.Forms.Controls;
using Eto.Wpf.Forms.Menu;
using Eto.Wpf.Forms.ToolBar;
#endif

#if OS_LINUX || OS_FREEBSD
using System;
using Eto.GtkSharp;
using Eto.GtkSharp.Drawing;
using Eto.GtkSharp.Forms.Controls;
using Eto.GtkSharp.Forms.Menu;
using Eto.GtkSharp.Forms.ToolBar;
using Gtk;
#endif

#if OS_MACOS
using Eto.Mac.Drawing;
using Eto.Mac.Forms.Controls;
using Eto.Mac.Forms.Menu;
using Eto.Mac.Forms.ToolBar;
#endif

namespace GKUI.Platform
{
#if OS_MSWIN

    public class GKToolBarHandler : ToolBarHandler, GKToolBar.IHandler
    {
        public Color BackgroundColor
        {
            get { return Control.Background.ToEtoColor(); }
            set { Control.Background = value.ToWpfBrush(Control.Background); }
        }

        static readonly object FontKey = new object();

        public Font Font
        {
            get {
                return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control)));
            }
            set {
                if (Widget.Properties.Get<Font>(FontKey) != value) {
                    Widget.Properties[FontKey] = value;
                    if (value != null) {
                        ((FontHandler)value.Handler).Apply(Control, null);
                    }
                }
            }
        }

        public Color TextColor
        {
            get { return Control.Foreground.ToEtoColor(); }
            set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
        }
    }


    public class GKContextMenuHandler : ContextMenuHandler, GKContextMenu.IHandler
    {
        static readonly object FontKey = new object();

        public Font Font
        {
            get {
                return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control)));
            }
            set {
                if (Widget.Properties.Get<Font>(FontKey) != value) {
                    Widget.Properties[FontKey] = value;
                    if (value != null) {
                        ((FontHandler)value.Handler).Apply(Control, null);
                    }
                }
            }
        }
    }


    public class GKMenuBarHandler : MenuBarHandler, GKMenuBar.IHandler
    {
        public Color BackgroundColor
        {
            get { return Control.Background.ToEtoColor(); }
            set { Control.Background = value.ToWpfBrush(Control.Background); }
        }

        static readonly object FontKey = new object();

        public Font Font
        {
            get {
                return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control)));
            }
            set {
                if (Widget.Properties.Get<Font>(FontKey) != value) {
                    Widget.Properties[FontKey] = value;
                    if (value != null) {
                        ((FontHandler)value.Handler).Apply(Control, null);
                    }
                }
            }
        }

        public Color TextColor
        {
            get { return Control.Foreground.ToEtoColor(); }
            set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
        }
    }


    public class GKTabControlHandler : TabControlHandler, GKTabControl.IHandler
    {
        static readonly object FontKey = new object();

        public Font Font
        {
            get {
                return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control)));
            }
            set {
                if (Widget.Properties.Get<Font>(FontKey) != value) {
                    Widget.Properties[FontKey] = value;
                    if (value != null) {
                        ((FontHandler)value.Handler).Apply(Control, null);
                    }
                }
            }
        }
    }


    public class GKButtonToolItemHandler : ButtonToolItemHandler
    {
    }

#endif

#if OS_LINUX || OS_FREEBSD

    public class GKToolBarHandler : ToolBarHandler, GKToolBar.IHandler
    {
        public Color BackgroundColor
        {
            get { return Control.GetBase(); }
            set { Control.SetBase(value); }
        }

        public Font Font
        {
            get { return null; }
            set { }
        }

        public Color TextColor
        {
            get { return Control.GetForeground(); }
            set { Control.SetForeground(value); }
        }
    }


    public class GKContextMenuHandler : ContextMenuHandler, GKContextMenu.IHandler
    {
        public Font Font
        {
            get { return null; }
            set { }
        }
    }


    public class GKMenuBarHandler : MenuBarHandler, GKMenuBar.IHandler
    {
        public Color BackgroundColor
        {
            get { return Control.GetBase(); }
            set { Control.SetBase(value); }
        }

        public Font Font
        {
            get { return null; }
            set { }
        }

        public Color TextColor
        {
            get { return Control.GetForeground(); }
            set { Control.SetForeground(value); }
        }
    }


    public class GKTabControlHandler : TabControlHandler, GKTabControl.IHandler
    {
        /*public Font Font
        {
            get { return null; }
            set { }
        }*/
    }


    public class GKButtonToolItemHandler : ToolItemHandler<Gtk.ToolButton, GKButtonToolItem>, GKButtonToolItem.IHandler
    {
        Eto.Drawing.Image image;

        protected virtual void SetImage()
        {
            if (Control is Gtk.ToolButton button) {
                Console.WriteLine("SetImage()");
                if (image != null) {
                    GtkImage = image.ToGtk(Gtk.IconSize.Button);
                    GtkImage.Show();
                } else {
                    GtkImage = null;
                }
                button.IconWidget = GtkImage;
            }
        }

        public new Eto.Drawing.Image Image
        {
            get { return image; }
            set {
                if (image != value) {
                    image = value;
                    SetImage();
                }
            }
        }


        protected class GKButtonToolItemHandlerConnector : WeakConnector
        {
            public new GKButtonToolItemHandler Handler => (GKButtonToolItemHandler)base.Handler;

            public void HandleClicked(object sender, EventArgs e)
            {
                Handler?.Widget.OnClick(e);
            }
        }

        protected new GKButtonToolItemHandlerConnector Connector => (GKButtonToolItemHandlerConnector)base.Connector;

        public override void CreateControl(ToolBarHandler handler, int index)
        {
            Toolbar toolbar = handler.Control;
            base.Control = new ToolButton(base.GtkImage, base.Text);
            base.Control.IsImportant = true;
            base.Control.Sensitive = base.Enabled;
            base.Control.TooltipText = ToolTip;
            base.Control.ShowAll();
            base.Control.NoShowAll = true;
            base.Control.Visible = base.Visible;
            toolbar.Insert(base.Control, index);
            base.Control.Clicked += Connector.HandleClicked;
        }

        protected override WeakConnector CreateConnector()
        {
            return new GKButtonToolItemHandlerConnector();
        }
    }

#endif

#if OS_MACOS

    public class GKFontHandler : FontHandler, Font.IHandler
    {
        private static readonly Dictionary<string, int> fFontRefCounts = new Dictionary<string, int>();
        private static readonly object fLock = new object();
        private string fFontKey;

        private string GetFontKey()
        {
            if (fFontKey == null) {
                try {
                    var widget = Widget;
                    if (widget != null) {
                        fFontKey = $"{widget.FamilyName}|{widget.Size:F1}|{widget.FontStyle}";
                    }
                } catch {
                    // Widget may not be initialized yet
                }
            }
            return fFontKey;
        }

        internal static void AddRef(Font font)
        {
            if (font == null) return;
            string key = $"{font.FamilyName}|{font.Size:F1}|{font.FontStyle}";
            lock (fLock) {
                fFontRefCounts.TryGetValue(key, out int count);
                fFontRefCounts[key] = count + 1;
            }
        }

        internal static void Release(Font font)
        {
            if (font == null) return;
            string key = $"{font.FamilyName}|{font.Size:F1}|{font.FontStyle}";
            lock (fLock) {
                if (fFontRefCounts.TryGetValue(key, out int count)) {
                    if (count <= 1) {
                        fFontRefCounts.Remove(key);
                    } else {
                        fFontRefCounts[key] = count - 1;
                    }
                }
            }
        }

        /// <remarks>
        /// When Font is disposed it disposes underlying handler, which in turn disposes system NSFont.
        /// This renders all fonts using the same font family to become unusable.
        /// Only allow disposal when no other controls reference the same font family+size+style.
        /// </remarks>
        protected override bool DisposeControl
        {
            get {
                string key = GetFontKey();
                if (key == null) return false;

                lock (fLock) {
                    fFontRefCounts.TryGetValue(key, out int count);
                    return count <= 0;
                }
            }
        }
    }

    public class GKToolBarHandler : ToolBarHandler, GKToolBar.IHandler
    {
        public Color BackgroundColor
        {
            get { return SystemColors.Control; }
            set { }
        }

        public Font Font
        {
            get { return SystemFonts.Default(); }
            set { }
        }

        public Color TextColor
        {
            get { return SystemColors.ControlText; }
            set { }
        }
    }


    public class GKContextMenuHandler : ContextMenuHandler, GKContextMenu.IHandler
    {
        public Font Font
        {
            get { return SystemFonts.Default(); }
            set { }
        }
    }


    public class GKMenuBarHandler : MenuBarHandler, GKMenuBar.IHandler
    {
        public Color BackgroundColor
        {
            get { return SystemColors.Control; }
            set { }
        }

        public Font Font
        {
            get { return SystemFonts.Default(); }
            set { }
        }

        public Color TextColor
        {
            get { return SystemColors.ControlText; }
            set { }
        }
    }


    public class GKTabControlHandler : TabControlHandler, GKTabControl.IHandler
    {
        public Font Font
        {
            get { return SystemFonts.Default(); }
            set { }
        }
    }


    public class GKButtonToolItemHandler : ButtonToolItemHandler
    {
    }

#endif
}
