<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SendMail.aspx.cs" Inherits="WebApplication1.Admin.SendMail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
    .textEntry
    {}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="SearchPanel" runat="server">
        <div class="formPanel">
            <fieldset>
                <legend>Add Item</legend>
                <p>
                    <asp:Label ID="Label1" runat="server" Text="UserName" AssociatedControlID="UserName_tbx"></asp:Label>
                    <asp:TextBox ID="UserName_tbx" runat="server" CssClass="textEntry"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="UserName_tbx" ErrorMessage="RequiredFieldValidator"></asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="Label2" runat="server" AssociatedControlID="Title_tbx" 
                        Text="Title"></asp:Label>
                    <asp:TextBox ID="Title_tbx" runat="server" CssClass="textEntry"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="UserName_tbx" ErrorMessage="RequiredFieldValidator"></asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="Label3" runat="server" AssociatedControlID="Content_tbx" 
                        style="margin-right: 2px" Text="Content"></asp:Label>
                    <asp:TextBox ID="Content_tbx" runat="server" CssClass="textEntry" Height="94px" 
                        TextMode="MultiLine" Width="327px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                        ControlToValidate="UserName_tbx" ErrorMessage="RequiredFieldValidator"></asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="Error_lbl" runat="server" Text="Label"></asp:Label>
                </p>
                <p>
                    <asp:Button ID="SendMail_btn" runat="server" Text="SendMail" 
                        onclick="SendMail_btn_Click" />
                </p>
            </fieldset>
        </div>
    </asp:Panel>
</asp:Content>
