   M S B A M L   `   `   `  ���� W  SPointel.Interactions.Contact, Version=5.0.3.8, Culture=neutral, PublicKeyToken=null]3clr-namespace:Pointel.Interactions.Contact.Controls%Pointel.Interactions.Contact.Controls  <    6Pointel.Interactions.Contact.Controls.ContactDirectory   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6my-http://schemas.microsoft.com/wpf/2008/toolkit  :d2http://schemas.microsoft.com/expression/blend/2008  APointel3clr-namespace:Pointel.Interactions.Contact.Controls  I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  -   $	��300��  � 	Resources   �� MergedDictionaries �� Source��  �� $T N/Pointel.Interactions.Contact;component/Resource/StylesResourceDictionary.xaml?�%    .��� .+��� -   $��
BrdOpacity�����#4a4a4a K� ZIndex$ 1�� J� Opacity$	 0.1Q�����  �� ElementName$ userControlWorkbin�� �� Path$ Width����� $ userControlWorkbin��$ Height��� -   $��gridPreload��$ 1��$��	0,110,0,0q�.+��� -   $��
ImgPreload��$	��100��$	��100��$��Center=�$��Center=�"  WpfAnimatedGif.ImageBehavior   AnimatedSource$= 7/Agent.Interaction.Desktop;component/Images/Loading.gif���  � ColumnDefinitions �� $��* ��� -   	 �Name$	 grdColn_Seperator��$��3 ��� -   $	 grdColn_Information��$��0 �.+�]� -   $��ContactDirectoryView��.+�� -   $��GridSize��
 � RowDefinitions
 �� -	   $	 	grMessage��$��0 ��� $��30 ��� -
   $	 search��$��0 ��� $��* ��� -   $	 DetailsGrid��$��0 �.+�� $
��Auto��$
��Auto��$��0��
������� �� $��30 ��� -   $	 grdErrorColumn��$��* ��� $��30 �.+��� $;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��0��$��5q�$��20��$��20���� -   $��
txtMessage��$��1��$
��Wrap=�$��Center=�$��5q�Contact server is not active.�� -   $��btnCloseError��$��Right=�$��2��$��20��$��20��   NormalImageButton#��[  $��Center=�$��4q�.���� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png���� $��1��$
��Auto��$��2q����#D6D7D6$��1q�
������ ._�� $
��Auto�� � ShowGridLines .   �� $��0 ��� $��0 ��� $��* ��� $��25 �
 �� $
��Auto ��� $
��Auto ��� $��1 ��� $��9* �.+��� -   $��brdSearchBar��$��2��$��1��$��25�����#adaaad$	��0.5q�._�� -   $��grdQuickSearch�� �� $��* ��� $
��Auto �.+��� -   $��	txtSearch�� �� 	UndoLimit$ 10��$��0��$��0,0,2,3q�
������ 
 � Tag$ search��$��Stretch=�$��Stretch=�  MyWaterMarkStyle1#��[ .�� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� -   $��searchHeading��$��search��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��searchContent��$>��8Type text and press search button to search for the text��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��.���� -    ��Name$ btnAdvanceSearchResponse��$��1��$��2q�#��[   � Cursor$
 Hand��
�������$
��Left=�$��21��$��21�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� -   $��AdvanceSearchHeading��$��Search��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��AdvanceSearchContent��$>��8Type text and press search button to search for the text��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��.���� -   $��imgAdvanceSearchContact��$M��G/Agent.Interaction.Desktop;component/Images/Contact/SearchButton-01.png��$��20��$��20��$��1���� -   $ btnAdvanceSearch��#��[  $
 Hand��$��3��$��Center=�$��Center=�$��1��$��23��$��20��.���� -    ��Name$ AdvanceSearchImage��$I��C/Agent.Interaction.Desktop;component/Images/Contact/DoubleArrow.png��$��UniformToFill=�$��22��$��19�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� -   $��ShowAdvanceSearchHeading��$��Advance search��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��ShowAdvanceSearchContent��$!��Show advance search options��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -   $��grbAdvanceSearch��$��2q�$��2��$��Advanced Search��$
��Auto�����#ADAAAD$	��0.5q�$
��Auto=��� -   $��brdMain��$��3��$��2q����#D6D7D6$��1q�._�� 
 �� $��33 ��� -   $	 
ExpandGrid��$	��175��$��* ��� -   $	 HideAddresses��$��0 �.+��� $��0��$��2q�$
��Auto�����#D6D7D6$��1q�
������ ._�� .+���  �� Orientation$ 
Horizontal=�$��Right=�.+��� -    $ btnAddContact��#��[  $��23��$��23��$��3,2,3,2q�.���� $��0��$��3��$��22��$��22��$H��B/Agent.Interaction.Desktop;component/Images/Contact/ContactAdd.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� $��Create Contact��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $.��(Create new contact in contact directory.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -!   $ btnDeleteContact��#��[  $��3,2,3,2q�.���� $��0��$��3��$��22��$��22��$K��E/Agent.Interaction.Desktop;component/Images/Contact/DeleteContact.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� $��Delete Contact��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -"   $��txtDeleteContactToolTip��$5��/Delete selected contact from contact directory.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -#   $ btnMergeContact��#��[  $��3,2,3,2q�.���� $��0��$��3��$��22��$��22��$J��D/Agent.Interaction.Desktop;component/Images/Contact/MergeContact.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� $��Merge Contact��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $6��0Merge the selected contact with another contact.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -$   $ btnUnMergeContact��#��[  $��3,2,3,2q�.���� $��0��$��3��$��22��$��22��$L��F/Agent.Interaction.Desktop;component/Images/Contact/UnMergeContact.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� $��Un-Merge Contact��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $$��Un-Merge the selected contact.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -%   $ btnShowContactInfo��#��[  $��23��$��23��$��3,2,3,2q�.���� -&   $ imgContactInfoShow��$��0��$��3��$P��J/Agent.Interaction.Desktop;component/Images/Contact/ShowContactDetails.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� -'   $��ShowContactInfoHeading��$��Show Details��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -(   $��ShowContactInfoContacent��$+��%Show the details to selected contact.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��� -)   $$��grd_ContactInformationDatagrid��$��2q�$��1��
 �� $��* ��� $
��Auto �.+�� -*   $��grdContactView��$��0��.+�)  #Microsoft.Windows.Controls.DataGrid  -+   $��dgContactDirectory��
�������$��Stretch=�$��Stretch=�$
��Left=�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     RowHeaderWidth$ 0Q�   CanUserDeleteRows .     CanUserSortColumns .  $��2q����� *
Contacts+���� $ grdContactView��$ ActualWidth� � ContextMenu ��   ContextMenu#��[    Columns 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto    Header$ 
First Name��   MinWidth$ 50Q�    CellTemplate  �� .R��� $ 
Horizontal=�.+��� $y�	Collapsed=����� *	ContactId+�� $��4,4,0,4q����� *
First Name+  $
 Auto $ 	Last Name��$ 50Q�  �� .R��� $ 
Horizontal=�.+��� $��4,4,0,4q����� *	Last Name+  $
 Auto $ Phone Number��$ 50Q�  �� .R��� $ 
Horizontal=�.+�>   8Pointel.Interactions.Contact.Controls.ButtonWithDropDown  >   8Pointel.Interactions.Contact.Controls.ButtonWithDropDown!   
ValueClick ! ButtonWithDropDownCall_Click"   
PrefixText$" Call to ��#   Icon$C# =/Agent.Interaction.Desktop;component/Images/Contact/Voice.png��$��1q�$��22��$��32��$   Values$ �� *Phone Number+y��� *CallVisibility+�� $��1,4,0,4q����� *PhoneNoToDisplay+  $ * $ E-mail Address��$	 100Q�  �� .R��� $ 
Horizontal=�.+�  %! !ButtonWithDropDownSendEmail_Click$" 
E-mail to ��$K# E/Agent.Interaction.Desktop;component/Images/Contact/Email compose.png��$��32��$��32��$ �� *E-mail Address+y��� *EmailVisibility+�� $��1,4,0,4q����� *EmailToDisplay+    RowMenu% � DataContext& �� RelativeSource' m� Icon��%"  &	       7  �� -,   8��  % �� *PlacementTarget.DataContext+& �� *Self+.@�m� $��mnuAddTo��--   $��	Add to TO��' �� $��
mnuToImage��$?��9/Agent.Interaction.Desktop;component/Images/Email/Add.png��$��15��$��15��m� $��mnuAddCC��-.   $��
Add to CC.��' �� $��
mnuCcImage��$?��9/Agent.Interaction.Desktop;component/Images/Email/Add.png��$��15��$��15��m� $��	mnuAddBCC��-/   $��
Add to BCC��' �� $��mnuBccImage��$C��=/Agent.Interaction.Desktop;component/Images/Email/Done-01.png��$��15��$��15��(   RowStyle( �� ) �� 
TargetType,  &Microsoft.Windows.Controls.DataGridRow")  .��� * �� Property* �@�ContextMenu+ �� Value#+ [ , �� Triggers, P� - P� Property- �@ 
IsSelected. P� Value. . . ��� * � ��  HighlightedBrush#+ [ �� -0   $��txtError��$��No contacts to display��$y�	Collapsed=�$	��Top=�$��Center=�$��30q��� -1   $��	bdrPaging��$��1��$��30�����#D6D7D6$��1,0,1,1q�
������ ._��  �� $
��Auto ��� $��* �.+��� $��0��$ 
Horizontal=�$
��Left=�.+��� -2   $��btnFirst��#��[  $��20��.���� $��10��$��10��$J��D/Agent.Interaction.Desktop;component/Images/Contact/FirstPage-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -3   $��FirstHeading��$��
First Page��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -4   $��btnPrevious��#��[  $��20��.���� -5   $ btnPage��$��10��$��10��$M��G/Agent.Interaction.Desktop;component/Images/Contact/PreviousPage-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -6   $��PreviousHeading��$��Previous Page��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $ 
Horizontal=�$��Center=�.+��� $�� Page ���� -7   $��txtCurrentPage���� $
�� of ���� -8   $��txtTotalPage���� $�� Pages ���� -9   $��btnNext��#��[  $��20��.���� $��10��$��10��$J��D/Agent.Interaction.Desktop;component/Images/Contact/NextPage-01.png �� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -:   $��NextHeading��$��	Next Page��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -;   $��btnLastIndex��$y�	Collapsed=�#��[  $��20��.���� $��10��$��10��$I��C/Agent.Interaction.Desktop;component/Images/Contact/LastPage-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -<   $��LastHeading��$��	Last Page��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$��Center=�$ 
Horizontal=�$��Right=�$��0,0,2,0q�.+��� $��Items per page ��$��Center=��� -=   $��cmbItemsPerPage��/ �� SelectedIndex$/ 0��$y�Visible=�$��Center=�0 �� Text$0  ��$��5��  ComboBox#��[ $��0,0,25,0q����� *AvailableDirPageSizes+�� ->   $��txtStart��$��Center=��� $	�� - ��$��Center=��� -?   $��txtEnd��$��Center=��� $
�� of ��$��Center=��� -@   $��txtTotalItems��$��Center=��� $��
 Contacts ��$��Center=��� $��2��$��2q�$
��Auto�����#D6D7D6$��1q�
������ ._�� .+�� -A   $��gridEmailAddress�� .   �� $��75 ��� $��* �
 �� $��25 ��� $��25 ��� $��25 �.+��� -B   $��btnTo��$��0��$�� 0��$��2q�$��To��  NormalButton#��[ �� -C   $��txtTo��$ 10��$��0��$	�� 25��$��1��$�� ���� -D   $��btnCc��$��1��$�� 0��$��2q�$��Cc��#��[ �� -E   $��txtCc��$ 10��$��1��$	�� 25��$��1��$�� ���� -F   $��btnBcc��$��2��$�� 0��$��2q�$	��Bcc��#��[ �� -G   $��txtBcc��$ 10��$��2��$��25��$��1��$�� ��� -H   $��gridMergeContact�� .   �� $
��Auto ��� $��* �
 �� $��25 ��� $��25 �.+��� $��Description: ���� -I   $��txtDescription��$ 10��$��0��$��2q�$	�� 25��$��1��$�� ���� $��1��$��Reason: ���� -J   $��	txtReason��$ 10��$	�� 25��$��2q�$��1��$��1��$�� ��]� -K   $��
dockBottom��$��4��$��2q�$��RightToLeft=�.+��� $ 
Horizontal=�$��5,0,0,0q�.+��� -L   $��	btnCancel��$��0��$�� 2��$��2q�$��23��$��80��$��Right=�$��Cancel��#��[ �� -M   $��btnOK��$��0��$�� 1��$��2q�$��23��$��80��$��Ok��#��[ ]� -N   $��contactInfo��$��2��$y�	Collapsed=�