using NHibernate.Mapping.Attributes;

namespace NHibernate.Test
{
    [Class(Table = "baseball_test")]
    internal class BaseballTest
    {
        [Id(Name = "Id"), Generator(Class = "native")]
        public virtual int Id { get; set; }
        [Property(Column = "team_name")]
        public virtual string Team { get; set; } = "";
        [Property]
        public virtual int Wins { get; set; }
    }
    //internal class BaseballTestMap : ClassMap<BaseballTest>
    //{
    //    public BaseballTestMap()
    //    {            
    //        Id(x => x.Id).GeneratedBy.Identity();
    //        Map(x => x.Team, "team_name");
    //        Map(x => x.Wins);
    //        Table("baseball_test");            
    //    }
    //}
}
