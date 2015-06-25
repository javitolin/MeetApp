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
    <asp:FileUpload ID="FileUpload1" runat="server" />
    <asp:FileUpload ID="FileUpload2" runat="server" />
    <asp:TextBox ID="minutesInput" runat="server">Minutes</asp:TextBox>
    <asp:TextBox ID="maxDaysInput" runat="server">Max Days</asp:TextBox>
<asp:Button ID="loadButton" runat="server" OnClick="loadButton_Click" Text="Go" />
</asp:Content>


<asp:Content ID="Content2" runat="server" contentplaceholderid="ContentPlaceHolder2">
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
</asp:Content>



