using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models
{

    public abstract class BaseTrello
    {
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string ViewColor { get; set; }

    }
    public partial class TrelloBoard : BaseTrello
    {
        public string Desc { get; set; }
        public object DescData { get; set; }
        public bool Closed { get; set; }
        public string IdOrganization { get; set; }
        public object IdEnterprise { get; set; }
        public object Limits { get; set; }
        public object Pinned { get; set; }
        public string ShortLink { get; set; }
        public List<object> PowerUps { get; set; }
        public DateTimeOffset? DateLastActivity { get; set; }
        public List<object> IdTags { get; set; }
        public object DatePluginDisable { get; set; }
        public string CreationMethod { get; set; }
        public object IxUpdate { get; set; }
        public bool EnterpriseOwned { get; set; }
        public object IdBoardSource { get; set; }
        public bool Starred { get; set; }
        public Uri Url { get; set; }
        public Prefs Prefs { get; set; }
        public bool Subscribed { get; set; }
        public LabelNames LabelNames { get; set; }
        public DateTimeOffset DateLastView { get; set; }
        public Uri ShortUrl { get; set; }
        public object TemplateGallery { get; set; }
        public List<object> PremiumFeatures { get; set; }
        public List<Membership> Memberships { get; set; }
    }

    public partial class LabelNames
    {
        public string Green { get; set; }
        public string Yellow { get; set; }
        public string Orange { get; set; }
        public string Red { get; set; }
        public string Purple { get; set; }
        public string Blue { get; set; }
        public string Sky { get; set; }
        public string Lime { get; set; }
        public string Pink { get; set; }
        public string Black { get; set; }
    }

    public partial class Membership
    {
        public string Id { get; set; }
        public string IdMember { get; set; }
        public MemberType MemberType { get; set; }
        public bool Unconfirmed { get; set; }
        public bool Deactivated { get; set; }
    }

    public partial class Prefs
    {
        public PermissionLevel PermissionLevel { get; set; }
        public bool HideVotes { get; set; }
        public Voting Voting { get; set; }
        public Comments Comments { get; set; }
        public Comments Invitations { get; set; }
        public bool SelfJoin { get; set; }
        public bool CardCovers { get; set; }
        public bool IsTemplate { get; set; }
        public CardAging CardAging { get; set; }
        public bool CalendarFeedEnabled { get; set; }
        public string Background { get; set; }
        public Uri BackgroundImage { get; set; }
        public List<BackgroundImageScaled> BackgroundImageScaled { get; set; }
        public bool BackgroundTile { get; set; }
        public BackgroundBrightness BackgroundBrightness { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundBottomColor { get; set; }
        public string BackgroundTopColor { get; set; }
        public bool CanBePublic { get; set; }
        public bool CanBeEnterprise { get; set; }
        public bool CanBeOrg { get; set; }
        public bool CanBePrivate { get; set; }
        public bool CanInvite { get; set; }
    }

    public partial class BackgroundImageScaled
    {
        public long Width { get; set; }
        public long Height { get; set; }
        public Uri Url { get; set; }
    }

    public enum MemberType { Admin, Normal };

    public enum BackgroundBrightness { Dark };

    public enum CardAging { Pirate, Regular };

    public enum Comments { Admins, Members };

    public enum PermissionLevel { Org, Private, Public };

    public enum Voting { Disabled };
}

public partial class TrelloList : BaseTrello
{
    public bool Closed { get; set; }
    public double Pos { get; set; }
    public object SoftLimit { get; set; }
    public string IdBoard { get; set; }
    public bool Subscribed { get; set; }
    public int QtdCards { get; set; }
}

public partial class TrelloLabel : BaseTrello
{
    public string IdBoard { get; set; }
    public string Color { get; set; }
}
public partial class TrelloCard : BaseTrello
{
    public object CheckItemStates { get; set; }
    public bool Closed { get; set; }
    public DateTime DateLastActivity { get; set; }
    public string Desc { get; set; }
    public DescData DescData { get; set; }
    public long? DueReminder { get; set; }
    public string IdBoard { get; set; }
    public string IdList { get; set; }
    public List<object> IdMembersVoted { get; set; }
    public long IdShort { get; set; }
    public string IdAttachmentCover { get; set; }
    public List<string> IdLabels { get; set; }
    public bool ManualCoverAttachment { get; set; }
    public double Pos { get; set; }
    public string ShortLink { get; set; }
    public bool IsTemplate { get; set; }
    public Badges Badges { get; set; }
    public bool DueComplete { get; set; }
    public DateTimeOffset? Due { get; set; }
    public List<object> IdChecklists { get; set; }
    public List<string> IdMembers { get; set; }
    public List<TrelloLabel> Labels { get; set; }
    public Uri ShortUrl { get; set; }
    public bool Subscribed { get; set; }
    public Uri Url { get; set; }
    public Cover Cover { get; set; }
}

public partial class Badges
{
    public AttachmentsByType AttachmentsByType { get; set; }
    public bool Location { get; set; }
    public long Votes { get; set; }
    public bool ViewingMemberVoted { get; set; }
    public bool Subscribed { get; set; }
    public string Fogbugz { get; set; }
    public long CheckItems { get; set; }
    public long CheckItemsChecked { get; set; }
    public object CheckItemsEarliestDue { get; set; }
    public long Comments { get; set; }
    public long Attachments { get; set; }
    public bool Description { get; set; }
    public DateTimeOffset? Due { get; set; }
    public bool DueComplete { get; set; }
}

public partial class AttachmentsByType
{
    public Trello Trello { get; set; }
}

public partial class Trello
{
    public long Board { get; set; }
    public long Card { get; set; }
}

public partial class Cover
{
    public string IdAttachment { get; set; }
    public object Color { get; set; }
    public object IdUploadedBackground { get; set; }
    public string Size { get; set; }
    public string Brightness { get; set; }
}

public partial class DescData
{
    public object Emoji { get; set; }
}


public partial class TrelloCardAttachment : BaseTrello
{
    public string Id { get; set; }
    public long? Bytes { get; set; }
    public DateTimeOffset Date { get; set; }
    public string EdgeColor { get; set; }
    public string IdMember { get; set; }
    public bool IsUpload { get; set; }
    public string MimeType { get; set; }
    public string Name { get; set; }
    public List<Preview> Previews { get; set; }
    public Uri Url { get; set; }
    public long Pos { get; set; }
    public string FileName { get; set; }
}

public partial class Preview
{
    public string PreviewId { get; set; }
    public string Id { get; set; }
    public bool Scaled { get; set; }
    public Uri Url { get; set; }
    public long Bytes { get; set; }
    public long Height { get; set; }
    public long Width { get; set; }
}

