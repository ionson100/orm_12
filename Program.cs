using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using ORM_1_21_;
using ORM_1_21_.Attribute;
using ORM_1_21_.Transaction;

namespace TestSimpleOrm
{
    class Program
    {
        static Program()
        {
#if DEBUG
            new Configure("Server=127.0.0.1;Port=5432;Database=testorm;User Id=postgres;Password=****;", ProviderName.Postgresql, "c:\\testorm.txt");
            return;
#endif
            new Configure("Server=127.0.0.1;Port=5432;Database=testorm;User Id=postgres;Password=****;", ProviderName.Postgresql, null);
        }
        static void Main(string[] args)
        {
            using (var ses=Configure.GetSession())
            {
                if (ses.TableExists<MyTableProduct>() == false)
                {
                    ses.TableCreate<MyTableProduct>();
                }

                if (!ses.Querion<MyTableProduct>().Any())
                {
                    for (int i = 0; i < 100; i++)
                    {
                        MyTableProduct myTableProduct = new MyTableProduct()
                        {
                            Name = $"name_{i}",
                            Price = new decimal(34.45),
                            IsTobacco = true,
                            Description = $"product name name_{i} price {new decimal(34.45)}"
                        };
                        ses.Save(myTableProduct);

                    }
                }

                var list = ses.Querion<MyTableProduct>().Where(a => a.IsNew == null).Limit(0,50).OrderBy(s=>s.DateCreate).ToList();
                var one = ses.Querion<MyTableProduct>().SingleOrDefault(a => a.Name.Contains("name_11"));
                decimal dd = new decimal(34.45);
                var val1= ses.FreeSql<MyTableProduct>("select * from product where price = @1 and is_tobacco = @2 order by data_create limit (10)", 
                    new Parameter("@1",dd),
                    new Parameter("@2",true)).ToList();
                string taleNameName = ses.TableName<MyTableProduct>();
                var val2=ses.Get<MyTableProduct>(new Guid("2455f03a-5eaf-455b-8c14-b49f9c04f521"));
                val2.Price=new decimal(100.23);
                ITransaction transaction = ses.BeginTransaction();
                try
                {
                    ses.Save(val2);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                   transaction.Rollback();
                }

                var val3 = ses.Get<MyTableProduct>(new Guid("2455f03a-5eaf-455b-8c14-b49f9c04f521")).Price;
                DataTable dataTable = ses.GetDataTable("select * from product");


            }
        }
    }

    [MapTableName("product")]
    class MyTableProduct
    {

        [MapPrimaryKey("id",Generator.Assigned)]
        public Guid Id { get; set; }= Guid.NewGuid();

        [MapColumnName("name")]
        public string Name { get; set; }


        [MapColumnName("is_tobacco")]
        public bool IsTobacco { get; set; }

        [MapColumnName("is_new")]
        [MapDefaultValue("NULL")]
        public bool? IsNew { get; set; }

        [MapColumnName("price")]
        public decimal Price { get; set; }


        [MapColumnName("my_guid")] 
        public Guid MyGuid { get; set; } = Guid.Empty;


        [MapColumnName("description")]
        [MapColumnType("TEXT")]
        public string Description { get; set; }

        [MapColumnName("data_create")]
        [MapIndex]
        public DateTime DateCreate { get; set; }=DateTime.Now;
    }
}
