   M S B A M L   `   `   `  ���� W  SPointel.Interactions.Contact, Version=5.0.3.8, Culture=neutral, PublicKeyToken=nulli9clr-namespace:Pointel.Interactions.Contact.CustomControls+Pointel.Interactions.Contact.CustomControls  a5clr-namespace:Pointel.Interactions.Contact.Converters'Pointel.Interactions.Contact.Converters  :    4Pointel.Interactions.Contact.Controls.ContactHistory   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  6my-http://schemas.microsoft.com/wpf/2008/toolkit  MCustomControl9clr-namespace:Pointel.Interactions.Contact.CustomControls  E	Converter5clr-namespace:Pointel.Interactions.Contact.Converters  -   $��Calibri)�$��12(�  � 	Resources   �� MergedDictionaries �� SourceL  FPointel.Interactions.Contact.Converters.UriUnescapeDataStringConverter   UriUnescapeDataString  MediaTemplateVoice  MediaTemplateEmailInbound  MediaTemplateEmailOutbound  MediaTemplateChat  MediaTemplateDefault��  �� $T N/Pointel.Interactions.Contact;component/Resource/StylesResourceDictionary.xaml?�%�  &	        &	      &	 w     &	 �     &	 m    &	 �      �� 
VoiceMedia��.R��� $��16��$��16��$C��=/Agent.Interaction.Desktop;component/Images/Contact/Voice.png���� EmailMediaInbound��.R��� $��16��$��16��$D��>/Agent.Interaction.Desktop;component/Images/Email/Inbound1.png���� EmailMediaOutbound��.R��� $��16��$��16��$E��?/Agent.Interaction.Desktop;component/Images/Email/Outbound2.png���� 	ChatMedia��.R��� $��16��$��16��$B��</Agent.Interaction.Desktop;component/Images/Contact/Chat.png���� DefaultMedia��.R��� $��16��$��16��$A��;/Agent.Interaction.Desktop;component/Images/Transparent.gif��.��� .+��� -   $��
BrdOpacity�����#666666 K� ZIndex$ 1�� J� Opacity$
 0.75Q��� -   $��gridPreload��$ 2��$��	0,110,0,0q�.+��� -   $��
ImgPreload��$��80��$��80��$��Center=�$��Center=�"  WpfAnimatedGif.ImageBehavior   AnimatedSource �� *PreloadImage+�� -   $��txtLoadingProgress��$��Center=�$��
Loading...��
�������$��14(�$��Center=�$��5q�� -   $��GridSize�� � ShowGridLines .  
�������$
��Auto=� � RowDefinitions �� -    �Name$ 	grMessage��$��0 ��� $��40 ��� -   $ grsearch��$��0 ��� $��30 ��� -	   $ detailsRowdef��$��* �.+�� $
��Auto��$
��Auto��$��0��
�������	 � ColumnDefinitions	 �� $��30 ��� -
   $ grdErrorColumn��$��* ��� $��30 �.+��� -   $��imgStatusImage��$;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��0��$��5q�$��20��$��20���� -   $��lblError��$��1��$
��Wrap=�$��Center=�$��5q�ECThe contact server is not active. Please conact your administrator.�� -   $��btnCloseError��$��Right=�$��2��$��20��$��20��  NormalImageButton#��[ $��Center=�$��4q�.���� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png���� $��1��$
��Auto��$��2q����#D6D7D6$��1q�
������ ._�� 	 �� $
��Auto ��� $��* ��� $
��Auto �.+��� 
 �� Orientation$
 
Horizontal=�$��2q�$��Stretch=�.+��� $
 Vertical=�.+��� $+��%Arch      All      1M      1W      1D��$	��Top=�$	��157��$��23��$��2,-4,0,0q��� $	��135��$��16��$��0,-5,0,0q�.+��� -   $��CustomSlider��  MyCustomStyleForSlider#��[  �� IsSnapToTickEnabled .  �� Ticks$ 0,25,50,75,100R�$��16��$	��140�� � Minimum$ 0Q� � SmallChange$ 25Q� � LargeChange$ 25Q� � Maximum$	 100Q��� -   $��btnDownArrowFilter��  numberButton#��[ $��2,0,2,0q�$��55��.��� .+��� -   $��lblMoreActionFilter��$��Center=�$	�� 35��$��Filter���� $��10��$��Center=�$C��=/Agent.Interaction.Desktop;component/Images/Contact/Arrow.png�� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��FilterHeading��$��Filter��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��FilterContent��$��Filter the interactions��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -   $��brdSearchBar��$��26�����#adaaad$	��0.5q�$��Stretch=�$��1��._�� 	 �� �� $
��Auto �.+��� -   $��	txtSearch�� �� 	UndoLimit$ 10��$��Stretch=�$��	1,1,0,0.5q�
 � Tag$ Quick search��$��80�� 	 MyWaterMarkStyle1#��[	 .�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��searchHeading��$��Search��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��searchContent��$X��RType date, status or subject and press search button to search for the interaction��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��.���� -    ��Name$ btnAdvanceSearchResponse��$��1��#��[ 
�������$��25��$
��Left=�$��23�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��AdvanceSearchHeading��$��Search��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��AdvanceSearchContent��$X��RType date, status or subject and press search button to search for the interaction��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��.���� -   $��imgAdvanceSearchContact��$M��G/Agent.Interaction.Desktop;component/Images/Contact/SearchButton-01.png��$��20��$��20��$��1,1,2,1q�$��1���� -   $��btnAdvanceSearch��#��[ $��2��$��3q�$��23��$��20��.���� -   $��AdvanceSearchImage��$I��C/Agent.Interaction.Desktop;component/Images/Contact/DoubleArrow.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��ShowAdvanceSearchHeading��$��Advanced Search��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��ShowAdvanceSearchContent��$"��Show advanced search option.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -   $��grbAdvanceSearch��$��2��$��Advanced Search��$��2q�$
��Auto�����#ADAAAD$	��0.5q�$
��Auto=��� $��3��$	��1.5q�$
��Auto�����#D6D7D6$��1q�
������ ._��  �� 	 �� -    $ gcOpen��$��0 ��� -!   $ gcReply��$��0 ��� -"   $ 
gcReplyAll��$��0 ��� -#   $ gcResend��$��0 ��� -$   $ gcCall��$��0 ��� $��* ��� $
��Auto ��� $
��Auto �.+��� -%   $ btnOpen��#��[ $��3,2,3,2q�.���� $��20��$J��D/Agent.Interaction.Desktop;component/Images/Contact/ReplyMail-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $"��Open selected mail in window��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -&   $ btnReply��#��[ $��0��$��1��$��3,2,3,2q�.���� -'    ��Name$ imgReply��$��0��$��1��$��20��$E��?/Agent.Interaction.Desktop;component/Images/Email/ReplyMail.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��
Reply Mail��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��Reply to selected mail.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -(   $ btnReplyAll��#��[ $��0��$��2��$��3,2,3,2q�.���� -)   $ imgReplyAll��$��0��$��2��$��20��$M��G/Agent.Interaction.Desktop;component/Images/Contact/ReplyAllMail-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��	Reply All��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $%��Reply to all for selected mail.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -*   $ 	btnReSend��#��[ $��3��$��3��$��3,2,3,2q�.���� -+   $ imgSende��$��0��$��2��$��20��$M��G/Agent.Interaction.Desktop;component/Images/Contact/ReplyAllMail-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Resend Mail��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $)��#Send a mail again to selected mail.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -,   $��
Btn_Export��$��5��$y�	Collapsed=�$��Right=�$��3,2,3,2q�#��[ ���� $��16��$��16��$D��>/Agent.Interaction.Desktop;component/Images/Contact/export.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Export Data��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $+��%Export the configured data into file.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200�� � ContextMenu ��  
 ContextMenu#��[
 .@�m� --   $	��PDF�� m� Icon �� $A��;/Agent.Interaction.Desktop;component/Images/Contact/pdf.png��$��15��$��15��m� -.   $��Excel�� �� $C��=/Agent.Interaction.Desktop;component/Images/Contact/excel.png��$��15��$��15��m� -/   $	��CSV�� �� $A��;/Agent.Interaction.Desktop;component/Images/Contact/CSV.png��$��15��$��15���� -0   $ btnContactExpand��#��[ $��6��$��Right=�$��3,2,3,2q�.���� -1   $ 
imgContact��$��0��$��3��$��22��$��22��$E��?/Agent.Interaction.Desktop;component/Images/Contact/Limited.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -2   $��ExpandHeading��$��Hide Details��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -3   $��ExpandContent��$+��%Hide details for selected interation.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -4   $��	DetailsSV��$��4��$
��Auto=�   �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value  VerticalScrollBar��%�   '��       7  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8   .���  �� -5   $ 
ExpandGrid��$	��145 ��� $��30 ��� -6   $ DetailsGrid��$��* �.+��� -7   $��brdContactHistory��$��2q�$��0�����#D6D7D6$��1q�._�� .+�)  #Microsoft.Windows.Controls.DataGrid  -8   $��dgContactHistory��
�������$��Stretch=�$��Stretch=�$	��100��$
��Left=�   HorizontalGridLinesBrush �#D6D7D6    VerticalGridLinesBrush  �#D6D7D6!   AutoGenerateColumns! . "   CanUserAddRows" .  #   RowHeaderWidth$# 0Q�$   CanUserDeleteRows$ .  %   
IsReadOnly% . &   CanUserSortColumns& .  $
��Auto=�$��2q����� ���� *Width+' �� ElementName$' brdContactHistory��(   RowStyle( �� ,  &Microsoft.Windows.Controls.DataGridRow"   P�  �@ 
IsSelected . . ���  � ��  HighlightedBrush# [ �� -9   $��txtError��$��Center=�$��No data found to display.��$y�	Collapsed=�$��Center=�$��10q��� -:   $��	brdPaging��$��1�����#D6D7D6$��2q�$��2,0,2,2q�
������ ._�� .+��� $��2,0,0,0q�$��1��$
 
Horizontal=�$
��Left=�.+��� -;   $��btnFirst��#��[ $��20��.���� $��10��$��10��$J��D/Agent.Interaction.Desktop;component/Images/Contact/FirstPage-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -<   $��FirstHeading��$��
First Page��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -=   $��btnPreviousPage��#��[ $��20��.���� $��10��$��10��$M��G/Agent.Interaction.Desktop;component/Images/Contact/PreviousPage-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� ->   $��PreviousHeading��$��Previous Page��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $
 
Horizontal=�$��Center=�.+��� -?   $��txtStart��$��Center=��� $	�� - ��$��Center=��� -@   $��txtEnd��$��Center=�$��5,0,0,0q��� -A   $��btnNextPage��#��[ $��20��.���� $��10��$��10��$I��C/Agent.Interaction.Desktop;component/Images/Contact/NextPage-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -B   $��NextHeading��$��	Next Page��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -C   #��[ $��0��$y�	Collapsed=�.���� -D   $ imgLastIndex��$��10��$��10��$I��C/Agent.Interaction.Desktop;component/Images/Contact/LastPage-01.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -E   $��LastHeading��$��	Last Page��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��0,0,2,0q�$��1��$��Center=�$
 
Horizontal=�$��Right=�.+��� $��Items per page ��$��Center=�$��5,0,0,0q��� -F   $��cmbItemsPerPage��) �� SelectedIndex$) 0��$y�Visible=�$��Center=�* �� Text$*  ��$��5��  ComboBox#��[ $��5,0,0,0q����� *AvailableDirPageSizes+�� -G   $��
txtDetails��$+��%Please select item to display details��$��2��$��Center=�$��Center=��� -H   $��tbShowInteraction��$��2��$��2q�
�������$	��150��.@��� $��Details��.���� -I   $��
pnlDetails��$
 Vertical=�.+��� -J   $��lblSubjectBold��$��Center=�$
��Bold"�$
��Auto���� -K   $��detailEmail��$y�	Collapsed=�$	��150��._�� 	 �� $
��Auto �$��50���� $��* � �� -L   $ RowFrom��$
��Auto ��� -M   $ RowTo��$
��Auto ��� -N   $ RowCC��$
��Auto ��� -O   $ RowBCC��$
��Auto ��� -P   $ RowState��$
��Auto ��� -Q   $ RowAttachment��$
��Auto ��� -R   $ 
RowContent��$��* �.+��� -S   $��lblFrom��$��5,0,0,0q�$��0��$��0��$
��Left=�$��Center=�$
��Bold"�$��From:��� $��0��$��1��	 �� $��* ��� $
��Auto �.+��� -T   $��txtFrom��$ 10��$��0��
������ 
������ $��0q�+ �� 
IsReadOnly+ . $
��Left=���Z� �� -U   $��txtEmailStartDate��$ 10��$��1��$��0,0,3,0q�
������ 
������ $��0q�+ . $��Right=���Z� �� -V   $��lblTo��$��5,0,0,0q�$��1��$��0��$
��Bold"�$	��To:��$
��Left=�$��Center=��� -W   $��txtTo��$ 10��$��15��$��1��$��1��
������ 
������ $��0q�+ . ��Z� �� -X   $��lblCc��$��5,0,0,0q�$
��Left=�$��2��$��0��$
��Bold"�$	��Cc:��$��Center=��� -Y   $��txtCc��$ 10��$��15��$��2��$��1��
������ 
������ $��0q�+ . ��Z� �� -Z   $��lblBCc��$��5,0,0,0q�$
��Left=�$��3��$��0��$
��Bold"�$
��BCc:��$��Center=��� -[   $��txtBCc��$ 10��$��15��$��3��$��1��
������ 
������ $��0q�+ . ��Z� �� -\   $��lblState��$��5,0,0,0q�$
��Left=�$��4��$
��Bold"�$��State:��$��Center=��� -]   $��txtState��$ 10��$��4��$��1��
������ 
������ $��0q�+ . ��Z� �� -^   $��attacScroll��$��5��$��2��$
��Auto��$��60��$��Stretch=���. $��Hidden=�$
��Auto=�.��� -_   $��wpAttachments��$
��Auto=�]� -`   $��dockEmailContent��$��6��$��2����. �� -a   $��
detailChat��$y�	Collapsed=�._�� 	 �� $
��Auto �$��50����  �� �� -b   $ RowChatState��.+�E   ?Pointel.Interactions.Contact.CustomControls.BindableRichTextBox  -c   ,  Name$, mainRTB��E   ?Pointel.Interactions.Contact.CustomControls.BindableRichTextBox-   TextChanged$- mainRTB_TextChanged��$��Stretch=�$��90��$	��150��+ . $��2q�. �� IsDocumentEnabled. . $
��Auto=�/ J� IsManipulationEnabled/ .  0 J� 	AllowDrop0 . $��1q����#D6D7D6{�. $��2��1   	Documents1 �� *RTBDocument+  2 �� Path3 �� RelativeSource��%�  '��       'J� O     '_� x     '�� �    7  �� " ��.���  �@�ToolTip �� $2 NavigateUri�3 �� *Self+�� " J�.���  �@��Margin$ 2q��� " _�.���  � �� �� .R��� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��
URLHeading��$��
Pushed URL��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��
URLContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8    �� �� -d   $��lblChatState��$��5,0,0,0q�$
��Left=�$��2��$
��Bold"�$��State:��$��Center=��� -e   $��txtChatState��$ 10��$��2��$��1��
������ 
������ $��0q�+ . ��Z� �� -f   $��detailVoice��$y�	Collapsed=�$	��150��._�� 	 �� $
��Auto ��� $��* � �� -g   $ RowDate��$
��Auto ��� -h   $ 
RowConatct��$
��Auto ��� -i   $ RowPhoneNumber��$
��Auto ��� -j   $ RowDuration��$
��Auto ��� -k   $ RowVoiceState��$
��Auto �.+��� $��5,0,0,0q�$
��Auto��$��0��$��0��$
��Left=�$��Center=�$
��Bold"�$
��Date���� -l   $��txtVoiceDate��$ 10��$
��Auto��$��0��$��1��
������ 
������ $��0q�+ . ��Z� �� $��5,0,0,0q�$
��Auto��$��1��$��0��$
��Bold"�$��Contact��$
��Left=�$��Center=��� -m   $��txtVoiceContact��$ 10��$
��Auto��$��1��$��1��
������ 
������ $��0q�+ . ��Z� �� $��5,0,0,0q�$
��Auto��$
��Left=�$��2��$��0��$
��Bold"�$��Phone Number��$��Center=��� -n   $��txtVoicePhoneNo��$ 10��$
��Auto��$��2��$��1��
������ 
������ $��0q�+ . ��Z� �� $��5,0,0,0q�$
��Auto��$
��Left=�$��3��$��0��$
��Bold"�$��Duration��$��Center=��� -o   $��txtVoiceDuration��$ 10��$
��Auto��$��3��$��1��
������ 
������ $��0q�+ . ��Z� �� -p   $��lblVoiceState��$��5,0,0,0q�$
��Left=�$��4��$
��Bold"�$��State:��$��Center=��� -q   $��txtVoiceState��$ 10��$��4��$��1��
������ 
������ $��0q�+ . ��Z� �� $
��Note��.��� -r   $��gridNote�� �� $��* �	 �� $��* �.+��� -s   $��txtNotes��$ 10��+ . $��0��$��0��$
��Auto=�$
��Auto=�4 �� TextWrapping$
4 Wrap=��� -t   $��tapCaseInfo��$��	Case Data��.���� -u   $��	CaseWhite��$��3q�._�� 
������� �� $��* ��� $
��Auto �	 �� $��* �.+�  -v   $��
dgCaseInfo��$
��Auto��$��0��$��Calibri)�$��12(�$��0��$	��150��
�������$��LeftToRight=�" . 5   EnableRowVirtualization5 . !6   EnableColumnVirtualization6 . 7   AreRowDetailsFrozen7 . 8   RowDetailsVisibilityMode$8 Visible=�9   ColumnWidth8  2Microsoft.Windows.Controls.DataGridLengthConverter$9 * % . $��Stretch=�$	��Top=�$
��Left=� �#D6D7D6  �#D6D7D6$
��Auto=�& . ! .  $# 0Q�( �� "   P�  �@ 
IsSelected . . ���  � ��# [ :   Columns: 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /	  )Microsoft.Windows.Controls.DataGridColumn; 	  Width$; * < 	  Header$	< Key��=   CellTemplate= �� .R��� $
 
Horizontal=�.+��� $��4,4,0,4q����� *Key+  $; * $< Value��= �� .R��� $
 
Horizontal=�.+��� $��4,4,0,4q����� *Value+> �� 	Converter#> [  �� -w   $��txtNoCaseData��$'��!No case data available to display��$��Center=�$��35q�]� 
�������$��1��$	��Top=�$��2q�.+��� -x   $��lblDispositionHeader��$	��Top=�$��Calibri)�$��12(�$��Bottom=�$
��Bold"�$��Disposition Code:�����	#E643AFC6�� -y   $��lblDisposition��$
��Left=�$��Calibri)�$��12(�$
��Bold"�$��DispositionCode:���� -z   $��txtDisposition��$
��Left=�$��Center=�$��Calibri)�$��12(�