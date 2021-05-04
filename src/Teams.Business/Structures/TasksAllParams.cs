namespace Teams.Business.Structures
{
    public struct TasksAllParams
    {
        public int SpCompletedTasks { get; private set; }
        public int SpUnCompletedTasks { get; private set; }
        public int TotalStoryPoints { get; private set; }
        public int QuantityСompletedTasks { get; private set; }
        public int QuantityUnСompletedTasks { get; private set; }
        public int TeamMemberTotalSp { get; private set; }
        public double StoryPointsInDay { get; private set; }

        private TasksAllParams (int spCompletedTasks,
            int spUnCompletedTasks,
            int totalStoryPoints,
            int quantityСompletedTasks,
            int quantityUnСompletedTasks,
            int teamMemberTotalSp,
            double storyPointsInDay = 0)
        {
            SpCompletedTasks = spCompletedTasks;
            SpUnCompletedTasks = spUnCompletedTasks;
            TotalStoryPoints = totalStoryPoints;
            QuantityСompletedTasks = quantityСompletedTasks;
            QuantityUnСompletedTasks = quantityUnСompletedTasks;
            TeamMemberTotalSp = teamMemberTotalSp;
            StoryPointsInDay = storyPointsInDay;
        }

        public static TasksAllParams Create(int spCompletedTasks,
            int spUnCompletedTasks,
            int totalStoryPoints,
            int quantityСompletedTasks,
            int quantityUnСompletedTasks,
            int teamMemberTotalSp,
            double storyPointsInDay = 0) => 
            new TasksAllParams(spCompletedTasks, 
                spUnCompletedTasks, 
                totalStoryPoints, 
                quantityСompletedTasks, 
                quantityUnСompletedTasks, 
                teamMemberTotalSp, 
                storyPointsInDay);

        public void AddStoryPointsInDay(double storyPointsInDay) => StoryPointsInDay = storyPointsInDay;
     
    }
}
