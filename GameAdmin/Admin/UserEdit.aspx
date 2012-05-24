<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UserEdit.aspx.cs" Inherits="WebApplication1.Admin.UserEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="SearchPanel" runat="server">
    <fieldset>
        <legend>Account Search</legend>
        <p>
            <asp:Label ID="Label1" runat="server" Text="UserName"></asp:Label>
            <asp:TextBox ID="UserName_tbx" runat="server"></asp:TextBox>           
        </p>
        <p>
         <asp:Button ID="Edit" runat="server" Text="Edit" OnClick="Edit_Click" />       
         </p>
    </fieldset>
    </asp:Panel>
    <asp:Panel ID="accountPanel" runat="server">
        <fieldset>
           
            <legend>Account information</legend>
            <p>
                <asp:FormView ID="FormView1" runat="server" DataSourceID="ObjectDataSource1">
                    <EditItemTemplate>
                        UserName:
                        <asp:TextBox ID="UserNameTextBox" runat="server" Text='<%# Bind("UserName") %>' />
                        <br />
                        NickName:
                        <asp:TextBox ID="NickNameTextBox" runat="server" Text='<%# Bind("NickName") %>' />
                        <br />
                        Sex:
                        <asp:CheckBox ID="SexCheckBox" runat="server" Checked='<%# Bind("Sex") %>' />
                        <br />
                        Gold:
                        <asp:TextBox ID="GoldTextBox" runat="server" Text='<%# Bind("Gold") %>' />
                        <br />
                        Money:
                        <asp:TextBox ID="MoneyTextBox" runat="server" Text='<%# Bind("Money") %>' />
                        <br />
                        PvePermission:
                        <asp:TextBox ID="PvePermissionTextBox" runat="server" Text='<%# Bind("PvePermission") %>' />
                        <br />
                        GiftToken:
                        <asp:TextBox ID="GiftTokenTextBox" runat="server" Text='<%# Bind("GiftToken") %>' />
                        <br />
                        <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                            Text="Update" />
                        &nbsp;<asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False"
                            CommandName="Cancel" Text="Cancel" />
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        ID:
                        <asp:TextBox ID="IDTextBox" runat="server" Text='<%# Bind("ID") %>' />
                        <br />
                        UserName:
                        <asp:TextBox ID="UserNameTextBox" runat="server" Text='<%# Bind("UserName") %>' />
                        <br />
                        NickName:
                        <asp:TextBox ID="NickNameTextBox" runat="server" Text='<%# Bind("NickName") %>' />
                        <br />
                        Sex:
                        <asp:CheckBox ID="SexCheckBox" runat="server" Checked='<%# Bind("Sex") %>' />
                        <br />
                        Attack:
                        <asp:TextBox ID="AttackTextBox" runat="server" Text='<%# Bind("Attack") %>' />
                        <br />
                        Defence:
                        <asp:TextBox ID="DefenceTextBox" runat="server" Text='<%# Bind("Defence") %>' />
                        <br />
                        Luck:
                        <asp:TextBox ID="LuckTextBox" runat="server" Text='<%# Bind("Luck") %>' />
                        <br />
                        Agility:
                        <asp:TextBox ID="AgilityTextBox" runat="server" Text='<%# Bind("Agility") %>' />
                        <br />
                        Gold:
                        <asp:TextBox ID="GoldTextBox" runat="server" Text='<%# Bind("Gold") %>' />
                        <br />
                        Money:
                        <asp:TextBox ID="MoneyTextBox" runat="server" Text='<%# Bind("Money") %>' />
                        <br />
                        Style:
                        <asp:TextBox ID="StyleTextBox" runat="server" Text='<%# Bind("Style") %>' />
                        <br />
                        Colors:
                        <asp:TextBox ID="ColorsTextBox" runat="server" Text='<%# Bind("Colors") %>' />
                        <br />
                        Hide:
                        <asp:TextBox ID="HideTextBox" runat="server" Text='<%# Bind("Hide") %>' />
                        <br />
                        Grade:
                        <asp:TextBox ID="GradeTextBox" runat="server" Text='<%# Bind("Grade") %>' />
                        <br />
                        GP:
                        <asp:TextBox ID="GPTextBox" runat="server" Text='<%# Bind("GP") %>' />
                        <br />
                        State:
                        <asp:TextBox ID="StateTextBox" runat="server" Text='<%# Bind("State") %>' />
                        <br />
                        ConsortiaID:
                        <asp:TextBox ID="ConsortiaIDTextBox" runat="server" Text='<%# Bind("ConsortiaID") %>' />
                        <br />
                        Repute:
                        <asp:TextBox ID="ReputeTextBox" runat="server" Text='<%# Bind("Repute") %>' />
                        <br />
                        ExpendDate:
                        <asp:TextBox ID="ExpendDateTextBox" runat="server" Text='<%# Bind("ExpendDate") %>' />
                        <br />
                        Offer:
                        <asp:TextBox ID="OfferTextBox" runat="server" Text='<%# Bind("Offer") %>' />
                        <br />
                        ConsortiaName:
                        <asp:TextBox ID="ConsortiaNameTextBox" runat="server" Text='<%# Bind("ConsortiaName") %>' />
                        <br />
                        Win:
                        <asp:TextBox ID="WinTextBox" runat="server" Text='<%# Bind("Win") %>' />
                        <br />
                        Total:
                        <asp:TextBox ID="TotalTextBox" runat="server" Text='<%# Bind("Total") %>' />
                        <br />
                        Escape:
                        <asp:TextBox ID="EscapeTextBox" runat="server" Text='<%# Bind("Escape") %>' />
                        <br />
                        Skin:
                        <asp:TextBox ID="SkinTextBox" runat="server" Text='<%# Bind("Skin") %>' />
                        <br />
                        IsConsortia:
                        <asp:CheckBox ID="IsConsortiaCheckBox" runat="server" Checked='<%# Bind("IsConsortia") %>' />
                        <br />
                        IsBanChat:
                        <asp:CheckBox ID="IsBanChatCheckBox" runat="server" Checked='<%# Bind("IsBanChat") %>' />
                        <br />
                        ReputeOffer:
                        <asp:TextBox ID="ReputeOfferTextBox" runat="server" Text='<%# Bind("ReputeOffer") %>' />
                        <br />
                        ConsortiaRepute:
                        <asp:TextBox ID="ConsortiaReputeTextBox" runat="server" Text='<%# Bind("ConsortiaRepute") %>' />
                        <br />
                        ConsortiaLevel:
                        <asp:TextBox ID="ConsortiaLevelTextBox" runat="server" Text='<%# Bind("ConsortiaLevel") %>' />
                        <br />
                        StoreLevel:
                        <asp:TextBox ID="StoreLevelTextBox" runat="server" Text='<%# Bind("StoreLevel") %>' />
                        <br />
                        ShopLevel:
                        <asp:TextBox ID="ShopLevelTextBox" runat="server" Text='<%# Bind("ShopLevel") %>' />
                        <br />
                        SmithLevel:
                        <asp:TextBox ID="SmithLevelTextBox" runat="server" Text='<%# Bind("SmithLevel") %>' />
                        <br />
                        ConsortiaHonor:
                        <asp:TextBox ID="ConsortiaHonorTextBox" runat="server" Text='<%# Bind("ConsortiaHonor") %>' />
                        <br />
                        ChairmanName:
                        <asp:TextBox ID="ChairmanNameTextBox" runat="server" Text='<%# Bind("ChairmanName") %>' />
                        <br />
                        AntiAddiction:
                        <asp:TextBox ID="AntiAddictionTextBox" runat="server" Text='<%# Bind("AntiAddiction") %>' />
                        <br />
                        AntiDate:
                        <asp:TextBox ID="AntiDateTextBox" runat="server" Text='<%# Bind("AntiDate") %>' />
                        <br />
                        RichesOffer:
                        <asp:TextBox ID="RichesOfferTextBox" runat="server" Text='<%# Bind("RichesOffer") %>' />
                        <br />
                        RichesRob:
                        <asp:TextBox ID="RichesRobTextBox" runat="server" Text='<%# Bind("RichesRob") %>' />
                        <br />
                        DutyLevel:
                        <asp:TextBox ID="DutyLevelTextBox" runat="server" Text='<%# Bind("DutyLevel") %>' />
                        <br />
                        DutyName:
                        <asp:TextBox ID="DutyNameTextBox" runat="server" Text='<%# Bind("DutyName") %>' />
                        <br />
                        Right:
                        <asp:TextBox ID="RightTextBox" runat="server" Text='<%# Bind("Right") %>' />
                        <br />
                        AddDayGP:
                        <asp:TextBox ID="AddDayGPTextBox" runat="server" Text='<%# Bind("AddDayGP") %>' />
                        <br />
                        AddWeekGP:
                        <asp:TextBox ID="AddWeekGPTextBox" runat="server" Text='<%# Bind("AddWeekGP") %>' />
                        <br />
                        AddDayOffer:
                        <asp:TextBox ID="AddDayOfferTextBox" runat="server" Text='<%# Bind("AddDayOffer") %>' />
                        <br />
                        AddWeekOffer:
                        <asp:TextBox ID="AddWeekOfferTextBox" runat="server" Text='<%# Bind("AddWeekOffer") %>' />
                        <br />
                        ConsortiaRiches:
                        <asp:TextBox ID="ConsortiaRichesTextBox" runat="server" Text='<%# Bind("ConsortiaRiches") %>' />
                        <br />
                        CheckCount:
                        <asp:TextBox ID="CheckCountTextBox" runat="server" Text='<%# Bind("CheckCount") %>' />
                        <br />
                        CheckCode:
                        <asp:TextBox ID="CheckCodeTextBox" runat="server" Text='<%# Bind("CheckCode") %>' />
                        <br />
                        CheckError:
                        <asp:TextBox ID="CheckErrorTextBox" runat="server" Text='<%# Bind("CheckError") %>' />
                        <br />
                        CheckDate:
                        <asp:TextBox ID="CheckDateTextBox" runat="server" Text='<%# Bind("CheckDate") %>' />
                        <br />
                        IsMarried:
                        <asp:CheckBox ID="IsMarriedCheckBox" runat="server" Checked='<%# Bind("IsMarried") %>' />
                        <br />
                        SpouseID:
                        <asp:TextBox ID="SpouseIDTextBox" runat="server" Text='<%# Bind("SpouseID") %>' />
                        <br />
                        SpouseName:
                        <asp:TextBox ID="SpouseNameTextBox" runat="server" Text='<%# Bind("SpouseName") %>' />
                        <br />
                        MarryInfoID:
                        <asp:TextBox ID="MarryInfoIDTextBox" runat="server" Text='<%# Bind("MarryInfoID") %>' />
                        <br />
                        IsLocked:
                        <asp:CheckBox ID="IsLockedCheckBox" runat="server" Checked='<%# Bind("IsLocked") %>' />
                        <br />
                        HasBagPassword:
                        <asp:CheckBox ID="HasBagPasswordCheckBox" runat="server" Checked='<%# Bind("HasBagPassword") %>' />
                        <br />
                        PasswordTwo:
                        <asp:TextBox ID="PasswordTwoTextBox" runat="server" Text='<%# Bind("PasswordTwo") %>' />
                        <br />
                        Password:
                        <asp:TextBox ID="PasswordTextBox" runat="server" Text='<%# Bind("Password") %>' />
                        <br />
                        DayLoginCount:
                        <asp:TextBox ID="DayLoginCountTextBox" runat="server" Text='<%# Bind("DayLoginCount") %>' />
                        <br />
                        IsCreatedMarryRoom:
                        <asp:CheckBox ID="IsCreatedMarryRoomCheckBox" runat="server" Checked='<%# Bind("IsCreatedMarryRoom") %>' />
                        <br />
                        Riches:
                        <asp:TextBox ID="RichesTextBox" runat="server" Text='<%# Bind("Riches") %>' />
                        <br />
                        SelfMarryRoomID:
                        <asp:TextBox ID="SelfMarryRoomIDTextBox" runat="server" Text='<%# Bind("SelfMarryRoomID") %>' />
                        <br />
                        IsGotRing:
                        <asp:CheckBox ID="IsGotRingCheckBox" runat="server" Checked='<%# Bind("IsGotRing") %>' />
                        <br />
                        Rename:
                        <asp:CheckBox ID="RenameCheckBox" runat="server" Checked='<%# Bind("Rename") %>' />
                        <br />
                        ConsortiaRename:
                        <asp:CheckBox ID="ConsortiaRenameCheckBox" runat="server" Checked='<%# Bind("ConsortiaRename") %>' />
                        <br />
                        Nimbus:
                        <asp:TextBox ID="NimbusTextBox" runat="server" Text='<%# Bind("Nimbus") %>' />
                        <br />
                        FightPower:
                        <asp:TextBox ID="FightPowerTextBox" runat="server" Text='<%# Bind("FightPower") %>' />
                        <br />
                        IsFirst:
                        <asp:TextBox ID="IsFirstTextBox" runat="server" Text='<%# Bind("IsFirst") %>' />
                        <br />
                        GiftToken:
                        <asp:TextBox ID="GiftTokenTextBox" runat="server" Text='<%# Bind("GiftToken") %>' />
                        <br />
                        LastAward:
                        <asp:TextBox ID="LastAwardTextBox" runat="server" Text='<%# Bind("LastAward") %>' />
                        <br />
                        QuestSite:
                        <asp:TextBox ID="QuestSiteTextBox" runat="server" Text='<%# Bind("QuestSite") %>' />
                        <br />
                        PvePermission:
                        <asp:TextBox ID="PvePermissionTextBox" runat="server" Text='<%# Bind("PvePermission") %>' />
                        <br />
                        PasswordQuest1:
                        <asp:TextBox ID="PasswordQuest1TextBox" runat="server" Text='<%# Bind("PasswordQuest1") %>' />
                        <br />
                        PasswordQuest2:
                        <asp:TextBox ID="PasswordQuest2TextBox" runat="server" Text='<%# Bind("PasswordQuest2") %>' />
                        <br />
                        FailedPasswordAttemptCount:
                        <asp:TextBox ID="FailedPasswordAttemptCountTextBox" runat="server" Text='<%# Bind("FailedPasswordAttemptCount") %>' />
                        <br />
                        AnswerSite:
                        <asp:TextBox ID="AnswerSiteTextBox" runat="server" Text='<%# Bind("AnswerSite") %>' />
                        <br />
                        IsDirty:
                        <asp:CheckBox ID="IsDirtyCheckBox" runat="server" Checked='<%# Bind("IsDirty") %>' />
                        <br />
                        <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
                            Text="Insert" />
                        &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False"
                            CommandName="Cancel" Text="Cancel" />
                    </InsertItemTemplate>
                    <ItemTemplate>
                        ID:
                        <asp:Label ID="IDLabel" runat="server" Text='<%# Bind("ID") %>' />
                        <br />
                        UserName:
                        <asp:Label ID="UserNameLabel" runat="server" Text='<%# Bind("UserName") %>' />
                        <br />
                        NickName:
                        <asp:Label ID="NickNameLabel" runat="server" Text='<%# Bind("NickName") %>' />
                        <br />
                        Sex:
                        <asp:CheckBox ID="SexCheckBox" runat="server" Checked='<%# Bind("Sex") %>' Enabled="false" />
                        <br />
                        Gold:
                        <asp:Label ID="GoldLabel" runat="server" Text='<%# Bind("Gold") %>' />
                        <br />
                        Money:
                        <asp:Label ID="MoneyLabel" runat="server" Text='<%# Bind("Money") %>' />
                        <br />
                        GiftToken:
                        <asp:Label ID="GiftTokenLabel" runat="server" Text='<%# Bind("GiftToken") %>' />
                        <br />
                        GP:
                        <asp:Label ID="GPLabel" runat="server" Text='<%# Bind("GP") %>' />
                        <br />
                        PvePermission:
                        <asp:Label ID="PvePermissionLabel" runat="server" Text='<%# Bind("PvePermission") %>' />
                          <br />
                        <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit"
                            Text="Edit" />
                    </ItemTemplate>
                </asp:FormView>
                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" DataObjectTypeName="SqlDataProvider.Data.PlayerInfo"
                    SelectMethod="GetUserSingleByUserName" 
                    TypeName="Bussiness.PlayerBussiness" UpdateMethod="UpdatePlayer"
                    OnSelecting="ObjectDataSource1_Selecting" 
                    onupdating="ObjectDataSource1_Updating">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="userName" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
               
                <asp:Label ID="error_lbl" runat="server" Text="Label" CssClass="failureNotification"></asp:Label>
                 <p>
                <asp:Button ID="Active_btn" runat="server" onclick="Active_btn_Click" 
                    Text="Active" />
                <asp:Button ID="DeActive_btn" runat="server" onclick="DeActive_btn_Click" 
                    Text="DeActive" />
                </p>
            </p>
        </fieldset>
         <asp:Button ID="Button1" runat="server" Text="Back" onclick="Button1_Click" />
    </asp:Panel>
</asp:Content>
