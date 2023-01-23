using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;
using DynamicData;
using WarpPointEditor.ViewModels;

namespace WarpPointEditor
{
    public class DockFactory : Factory
    {
        private readonly ShellViewModel _context;
        public static DocumentDock? Root => (Shell.Layout?.VisibleDockables?[0] as IDock)?.VisibleDockables?[0] as DocumentDock;
        public DockFactory(ShellViewModel context) => _context = context;

        public static Document AddDocument(Document document)
        {
            if (Root?.VisibleDockables?.Where(x => x.Id == document.Id).FirstOrDefault() is Document foundDocument) {
                document = foundDocument;
            }
            else {
                (Root?.VisibleDockables ?? throw new KeyNotFoundException("Could not find 'ActorDocuments' on the dock layout")).Add(document);
            }

            Root.ActiveDockable = document;
            return document;
        }

        public static void RemoveDocument(string id)
        {
            int index = (Root?.VisibleDockables?.Select(x => x.Id).IndexOf(id) ?? throw new NullReferenceException());
            if (index != -1) {
                Root?.VisibleDockables?.RemoveAt(index);
            }
        }

        public override IRootDock CreateLayout()
        {
            _context.Factory = this;

            var dockLayout = new DocumentDock() {
                Id = "ActorDocuments",
                Title = "Actor Documents",
                VisibleDockables = CreateList<IDockable>()
            };

            RootDock rootDock = new() {
                Id = "RootLayout",
                Title = "RootLayout",
                ActiveDockable = dockLayout,
                VisibleDockables = CreateList<IDockable>(dockLayout)
            };

            IRootDock root = CreateRootDock();
            root.Id = "RootDock";
            root.Title = "RootDock";
            root.ActiveDockable = rootDock;
            root.DefaultDockable = rootDock;
            root.VisibleDockables = CreateList<IDockable>(rootDock);

            _context.Layout = root;
            return (IRootDock)_context.Layout;
        }

        public override void InitLayout(IDockable layout)
        {
            HostWindowLocator = new Dictionary<string, Func<IHostWindow>> {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            base.InitLayout(layout);
        }
    }
}
