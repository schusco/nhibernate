using FluentNHibernate.Mapping;

namespace NHibernate.Test
{
    internal class BaseballTest
    {
        public virtual int Id { get; set; }
        public virtual string Team { get; set; } = "";
        public virtual int Wins { get; set; }
    }
    internal class BaseballTestMap : ClassMap<BaseballTest>
    {
        public BaseballTestMap()
        {            
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Team, "team_name");
            Map(x => x.Wins);
            Table("baseball_test");            
        }
    }
}
