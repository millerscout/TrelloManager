using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public partial class TrelloActions
    {
        public string Id { get; set; }
        public string IdMemberCreator { get; set; }
        public Data Data { get; set; }
        public TypeEnum Type { get; set; }
        public DateTime Date { get; set; }
        public object AppCreator { get; set; }
        public Limits Limits { get; set; }
        public MemberCreator MemberCreator { get; set; }
    }

    public partial class Data
    {
        public string Text { get; set; }
        public Card Card { get; set; }
        public Board Board { get; set; }
        public List List { get; set; }
        public Old Old { get; set; }
        public List ListBefore { get; set; }
        public List ListAfter { get; set; }
        public Attachment Attachment { get; set; }
    }

    public partial class Attachment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Uri Url { get; set; }
        public Uri PreviewUrl { get; set; }
        public Uri PreviewUrl2X { get; set; }
    }

    public partial class Board
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortLink { get; set; }
    }

    public partial class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? IdShort { get; set; }
        public string ShortLink { get; set; }
        public string Desc { get; set; }
        public double? Pos { get; set; }
        public string IdList { get; set; }
        public string IdAttachmentCover { get; set; }
        public Cover Cover { get; set; }
        public DateTimeOffset? Due { get; set; }
        public long? DueReminder { get; set; }
    }

    public partial class Cover
    {
        public object Color { get; set; }
        public string IdAttachment { get; set; }
        public object IdUploadedBackground { get; set; }
        public string Size { get; set; }
        public string Brightness { get; set; }
    }

    public partial class List
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public partial class Old
    {
        public string Desc { get; set; }
        public double? Pos { get; set; }
        public string IdList { get; set; }
        public string IdAttachmentCover { get; set; }
        public Cover Cover { get; set; }
        public object Due { get; set; }
        public object DueReminder { get; set; }
    }

    public partial class Limits
    {
        public Reactions Reactions { get; set; }
    }

    public partial class Reactions
    {
        public PerAction PerAction { get; set; }
        public PerAction UniquePerAction { get; set; }
    }

    public partial class PerAction
    {
        public string Status { get; set; }
        public long? DisableAt { get; set; }
        public long? WarnAt { get; set; }
    }

    public partial class MemberCreator
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public bool? ActivityBlocked { get; set; }
        public string AvatarHash { get; set; }
        public Uri AvatarUrl { get; set; }
        public string FullName { get; set; }
        public object IdMemberReferrer { get; set; }
        public string Initials { get; set; }
        public bool? NonPublicAvailable { get; set; }
    }

    public enum TypeEnum { CreateCard, CreateLabel, AddAttachmentToCard, AddChecklistToCard, AddLabelToCard, AddMemberToBoard, AddMemberToCard, AddMemberToOrganization, AddOrganizationToEnterprise, AddToEnterprisePluginWhitelist, AddToOrganizationBoard, CommentCard, ConvertToCardFromCheckItem, CopyBoard, CopyCard, CopyChecklist, CopyCommentCard, CreateBoard, CreateBoardInvitation, CreateBoardPreference, CreateList, CreateOrganization, CreateOrganizationInvitation, DeleteAttachmentFromCard, DeleteBoardInvitation, DeleteCard, DeleteCheckItem, DeleteLabel, DeleteOrganizationInvitation, DisableEnterprisePluginWhitelist, DisablePlugin, DisablePowerUp, EmailCard, EnableEnterprisePluginWhitelist, EnablePlugin, EnablePowerUp, MakeAdminOfBoard, MakeAdminOfOrganization, MakeNormalMemberOfBoard, MakeNormalMemberOfOrganization, MakeObserverOfBoard, MemberJoinedTrello, MoveCardFromBoard, MoveCardToBoard, MoveListFromBoard, MoveListToBoard, RemoveChecklistFromCard, RemoveFromEnterprisePluginWhitelist, RemoveFromOrganizationBoard, RemoveLabelFromCard, RemoveMemberFromBoard, RemoveMemberFromCard, RemoveMemberFromOrganization, RemoveOrganizationFromEnterprise, UnconfirmedBoardInvitation, UnconfirmedOrganizationInvitation, UpdateBoard, UpdateCard, UpdateCheckItem, UpdateCheckItemStateOnCard, UpdateChecklist, UpdateLabel, UpdateList, UpdateMember, UpdateOrganization, VoteOnCard };

}
