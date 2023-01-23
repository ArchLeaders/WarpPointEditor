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
        public static IDock Root => ((Shell.Layout?.ActiveDockable as IDock)?.ActiveDockable as IDock)!;
        public DockFactory(ShellViewModel context) => _context = context;

        public static (IDock dock, int index) CheckDockable(IDockable root, string id)
        {
            if (root is DocumentDock documentDock) {
                return (documentDock, documentDock.VisibleDockables?.Select(x => x.Id).IndexOf(id) ?? -1);
            }
            else if (root is ProportionalDock proportionalDock && proportionalDock.VisibleDockables != null) {
                foreach (var dockable in proportionalDock.VisibleDockables) {
                    var search = CheckDockable(dockable, id);
                    if (search.index >= 0) {
                        return search;
                    }
                }
            }

            return (Root, -1);
        }

        public static Document AddDocument(Document document)
        {
            (var dock, var index) = CheckDockable(Root, document.Id);
            if (index >= 0) {
                document = (dock.VisibleDockables![index] as Document)!;
            }
            else {
                Root.VisibleDockables!.Add(document);
            }

            dock.ActiveDockable = document;
            return document;
        }

        public override IRootDock CreateLayout()
        {
            _context.Factory = this;

            var dockLayout = new DocumentDock {
                Id = "ActorDocuments",
                Title = "Actor Documents",
                VisibleDockables = CreateList<IDockable>(new HomeViewModel())
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
