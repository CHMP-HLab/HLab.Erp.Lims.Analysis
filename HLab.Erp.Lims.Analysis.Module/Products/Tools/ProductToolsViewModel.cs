using System.Globalization;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products.Tools;

using H = H<ProductToolsViewModel>;

public class ProductToolsViewModel : ViewModel
{
    readonly IAclService _acl;
    readonly IDataService _data;
    public ProductToolsViewModel(IAclService acl, IDataService data)
    {
        _acl = acl;
        _data = data;
        H.Initialize(this);
    }

    public string Title => "{Product Tools}";

    public ICommand GenerateComponentsCommand { get; } = H.Command(c => c
        .Action((e, a) => e.GenerateComponents()));

    public string Message
    {
        get => _message.Get();
        set => _message.Set(value);
    }
    readonly IProperty<string> _message = H.Property<string>();

    async void GenerateComponents()
    {
        await foreach (var product in _data.FetchAsync<Product>())
        {
            string[] names;
            string[] variants;
            if (product.Name.Contains('+'))
            {
                names = product.Name.Split('+');
                variants = product.Variant.Split('+');
            }
            else
            {
                names = product.Name.Split('/');
                variants = product.Variant.Split('/');
            }

            if (names.Length != variants.Length)
            {
                Message += $"{product.Name} : {names.Length} != {variants.Length}\n";
                continue;
            }

            var unitMass = await _data.FetchOneAsync<UnitClass>(e => e.Symbol=="m");
            var unitVolume = await _data.FetchOneAsync<UnitClass>(e => e.Symbol=="v");

            var unitMassId = unitMass.Id;
            var unitVolumeId = unitVolume.Id;

            var g = await _data.FetchOneAsync<Unit>(e => unitMassId == e.UnitClassId && e.Exponent == -3);
            var mg = await _data.FetchOneAsync<Unit>(e => unitMassId == e.UnitClassId && e.Exponent == -6);
            var ml = await _data.FetchOneAsync<Unit>(e => unitVolumeId == e.UnitClassId && e.Exponent == -6);

            Message += $"{product.Name}\n";

            for(var i = 0; i<names.Length; i++)
            {
                var name = names[i].Trim();
                var variant = variants[i];

                var value = ""; 
                var dec = false;

                var n = 0;

                for (; n < variant.Length; n++)
                {
                    var c = variant[n];
                    if (c is >= '0' and <= '9')
                    {
                        value += c;
                        continue;
                    }

                    if (dec) break;

                    if (c is '.' or ',')
                    {
                        value += '.';
                        dec = true;
                        continue;
                    }

                    break;
                }

                var unitString = variant[n..].Trim();

                var inn = await _data.FetchOneAsync<Inn>(x => x.Name == name) ?? await _data.AddAsync<Inn>(e =>
                {
                    e.Name = name;
                });
                if (inn == null)
                {
                    Message += $"{name} could not be added\n";
                    continue;
                }

                Unit unit = unitString switch
                {
                    "g" => g,
                    "mg" => mg,
                    "ml" => ml,
                    _ => null
                };
                if (unit == null)
                {
                    Message += $"{unitString} not found in db\n";
                    continue;
                }

                if(double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var qty ))
                {
                    var pc = await _data.FetchOneAsync<ProductComponent>(e => 
                        e.ProductId == product.Id 
                        && e.InnId == inn.Id
                        );

                    if(pc==null)
                        await _data.AddAsync<ProductComponent>(e =>
                        {
                            e.Inn = inn;
                            e.Product = product;
                            e.Quantity = qty;
                            e.Unit = unit;
                        });

                    Message += $"-- {inn.Name} {qty} {unit.Symbol}\n";
                    continue;
                }
                else
                {
                    Message += $"{value} not parsed\n";
                    continue;
                }
            }

        }
    }
}