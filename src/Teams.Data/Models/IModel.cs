namespace Teams.Data.Models
{
    interface IModel<TDb> where TDb : class
    {
        public int Id { get; set; }
        public void Update(TDb model);
    }
}
