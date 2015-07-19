<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MeetApp._Default" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="ContentPlaceHolder1">
    <br />
    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
        Display="Dynamic" ControlToValidate="minutesInput" ErrorMessage="Minutes must be a number<br/>"
        ValidationExpression="^[0-9]+$" ForeColor="Red"></asp:RegularExpressionValidator>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
        Display="Dynamic" ControlToValidate="maxDaysInput" ErrorMessage="Max days must be a number<br/>"
        ValidationExpression="^[0-9]+$" ForeColor="Red"></asp:RegularExpressionValidator>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
        Display="Dynamic" ControlToValidate="FileUpload1"  ErrorMessage="File 1, please select .ics file"
        ValidationExpression="^.*(\.ics)$" ForeColor="Red"></asp:RegularExpressionValidator>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
        Display="Dynamic" ControlToValidate="FileUpload2"  ErrorMessage="File 2, please select .ics file"
        ValidationExpression="^.*(\.ics)$" ForeColor="Red"></asp:RegularExpressionValidator>
    <br />
    <asp:FileUpload ID="FileUpload1" runat="server" Visible="False" />
    <asp:FileUpload ID="FileUpload2" runat="server" Visible="False" />
    <asp:TextBox ID="minutesInput" runat="server">12</asp:TextBox>
    <asp:TextBox ID="maxDaysInput" runat="server">2</asp:TextBox>
<asp:Button ID="loadButton" runat="server" OnClick="loadButton_Click" Text="Go" />
</asp:Content>


<asp:Content ID="Content2" runat="server" contentplaceholderid="ContentPlaceHolder2">
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
</asp:Content>



<asp:Content ID="Content3" runat="server" contentplaceholderid="ContentPlaceHolder3">
    <asp:Button ID="googleLogin" runat="server" OnClick="googleLogin_Click" Text="Google test" CssClass="g-signin2" />
    <asp:Button ID="outlookButton" runat="server" OnClick="outlookButton_Click" Text="Outlook Test" />
    <asp:Label ID="tokenInfo" runat="server" Text="Label"></asp:Label>
    <asp:Literal ID="Literal2" runat="server"><br /></asp:Literal>
    <asp:Label ID="allCalendarItemsText" runat="server" Text="Label"></asp:Label>
</asp:Content>




