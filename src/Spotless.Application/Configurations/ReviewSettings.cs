namespace Spotless.Application.Configurations
{
    public class ReviewSettings
    {

        public const string SectionName = "Review";

        public int MaxCommentLength { get; set; } = 500;

        public int MinimumVisibleRating { get; set; } = 3;

        public bool RequiresAdminApproval { get; set; } = false;

        public int SubmissionCutoffHours { get; set; } = 72;
    }
}
