namespace MovieTicketBooking.Services;

public enum ScreenCreateResult
{
    Created,
    TheatreNotFound,
    DuplicateName
}

public enum ScreenUpdateResult
{
    Updated,
    ScreenNotFound,
    TheatreNotFound,
    DuplicateName
}

public enum ScreenDeleteResult
{
    Deleted,
    ScreenNotFound,
    HasDependencies
}