using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Mvvm;
using HLab.Network;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Data.Wpf
{
    using H = H<ConnectionDataViewModel>;

    public class ConnectionDataViewModel : ViewModel<ConnectionData>
    {
        private readonly IDataService _data;
        public ConnectionDataViewModel(IDataService data, IpScanner scanner)
        {
            _data = data;
            _scanner = scanner;
            H<ConnectionDataViewModel>.Initialize(this);

            _scanner.Scan(5432);
        }


        public async Task GetDatabases()
        {
            await foreach (var database in _data.GetDatabasesAsync(Model.Server, Model.UserName, Model.Password))
            {
                Databases.Add(database);
            }
        }

        public ObservableCollection<string> Databases { get; } = new();

        public string Server
        {
            get => _server.Get();
            set

            {
                if (_server.Set(value))
                {
                    Model.Server = value;
                    GetDatabases();
                }
            }
        }

        private readonly IProperty<string> _server = H<ConnectionDataViewModel>.Property<string>(c => c
            .Set(e => e.Model.Server)
            .On(e => e.Model.Server).Update());

        private IpScanner _scanner { get; }

        public ReadOnlyObservableCollection<string> Servers => _scanner.FoundServers;

        public ICommand OkCommand { get; } = H.Command(c => c
            .Action(e =>
            {
                
            }));
    }
}