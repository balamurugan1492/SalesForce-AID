   M S B A M L   `   `   `  ���� V  RPointel.Interaction.Workbin, Version=5.0.3.5, Culture=neutral, PublicKeyToken=null[2clr-namespace:Pointel.Interaction.Workbin.Controls$Pointel.Interaction.Workbin.Controls  Y1clr-namespace:Pointel.Interaction.Workbin.Helpers#Pointel.Interaction.Workbin.Helpers  �Wclr-namespace:Pointel.Interaction.Workbin.Controls;assembly=Pointel.Interaction.Workbin$Pointel.Interaction.Workbin.Controls  d `PresentationFramework.Classic, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35jMclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.ClassicMicrosoft.Windows.Themes =    7Pointel.Interaction.Workbin.Controls.WorkbinUserControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     @c2clr-namespace:Pointel.Interaction.Workbin.Controls     I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  6my-http://schemas.microsoft.com/wpf/2008/toolkit  <self1clr-namespace:Pointel.Interaction.Workbin.Helpers  epointelWclr-namespace:Pointel.Interaction.Workbin.Controls;assembly=Pointel.Interaction.Workbin  lMicrosoft_Windows_ThemesMclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Classic  -   $��userControlWorkbin��$��Calibri)�$��2q�$��12(�  J� CommandBindings  )  #System.Windows.Input.CommandBinding  -      Command$
 Open��  -   $ Delete�� � 	Resources  �� MergedDictionaries �� Source   MediaTemplateVoice  MediaTemplateEmailInbound  MediaTemplateEmailOutbound  MediaTemplateChat  MediaTemplateDefault��  �� $B </Pointel.Interaction.Workbin;component/Resource/Generic.xaml?�%|  &	        &	 r     &	 �     &	 h    &	 �    �� 
VoiceMedia��.R��� $��20��$��20��$C��=/Agent.Interaction.Desktop;component/Images/Contact/Voice.png���� EmailMediaInbound��.R��� $��20��$��20��$D��>/Agent.Interaction.Desktop;component/Images/Email/Inbound1.png���� EmailMediaOutbound��.R��� $��20��$��20��$E��?/Agent.Interaction.Desktop;component/Images/Email/Outbound2.png���� 	ChatMedia��.R��� $��20��$��20��$B��</Agent.Interaction.Desktop;component/Images/Contact/Chat.png���� DefaultMedia��.R��� $��20��$��20��$A��;/Agent.Interaction.Desktop;component/Images/Transparent.gif��.��� .+��� -   $��
BrdOpacity�� $� IsVirtualizing . ���#4a4a4a K� ZIndex$ 0�� J� Opacity$	 0.1Q�����  �� ElementName$ userControlWorkbin��	 �� Path$	 Width����� $ userControlWorkbin��$	 Height��� -   $��
ImgPreload��$ 0��$	��100��$	��100��$��Center=�$��Center=�"  WpfAnimatedGif.ImageBehavior
   AnimatedSource$=
 7/Agent.Interaction.Desktop;component/Images/Loading.gif��� .+�� -   $��gridCompleteContainer�� � ShowGridLines .  $��0�� � ColumnDefinitions �� $	��200��$	��200 �$	��200���� $��2 ��� $��* � � RowDefinitions �� .+��� $��0�����#D6D7D6$��1q�._�]� .+��� -   $��btnMyInteractionQueue��$��Bottom=�$y�	Collapsed=�$��My Interaction Queue��$��5q����#D9EBFC  WorkbinButton#��[ �� -   $��btnMyTeamWorkbin��$��Bottom=�$y�	Collapsed=�$��My Team Workbins��$��5q����#D9EBFC#��[ �� -	   $��btnMyWorkbin��$��Bottom=�$��
My Workbin��$��5q����#D9EBFC#��[ � $	��Top=�.+�R� -
   $��	tvWorkbin��$��2q�����  �� ItemsSource �� *Items+.R�� ���� *Color+.+��� $��treeImg��$
��Fill=�$I��C/Agent.Interaction.Desktop;component/Images/EMail/Email Workbin.png��$��0,0,1,0q�$��Center=�$��25��y��� *	Visible+�� $��Center=����� *Title+�� $��Center=����� *Count+y��� *	Visible+����  �� 
TargetType" Q�.���  �� Property �@Q�
IsSelected �� Value �� *
IsSelected+��  �@Q�
IsExpanded �� *
IsExpanded+ �� Triggers P�  P� Property �@Q�
IsSelected P� Value . . ���  � �� �#adc6e5��  � �� �#0575AF��  � ��$
 Bold"���  � ��$ 12(�]�  ]� 
Conditions ��  �� Property �@Q�
IsSelected �� Value . ��  �@Q�IsSelectionActive .   �� 	Resources ��%9   '��       �� " ��.���  �@��CornerRadius$ 2��  �� EndPoint �� 
StartPoint � Color � Offset �� Color��%�  (��       * SystemColors.HighlightBrushKey+)(�� V     *SystemColors.ControlBrushKey+)(�� �     *$"SystemColors.HighlightTextBrushKey+)(�� �     *" SystemColors.ControlTextBrushKey+)�� $	 0,1&�$	 0,0&�.��� $ 	#FFD9F4FF��$ 0Q�� $ 	#FF9BDDFB��$ 1Q��� $	 0,1&�$	 0,0&�.��� $ 	#FFEEEDED��$ 0Q�� $ 	#FFDDDDDD��$ 1Q��� $ Black���� $ Black��R� -   $��tvInteractionQueue��$y�	Collapsed=�$��2q�����  �� *Items+.R�� ���� *Color+.+��� $��0,0,5,5q�$��Center=�$��20��$��17������ *	ImagePath+�� $��0,0,0,5q�$��Center=����� *DisplayName+���� " Q�.���  �@Q�
IsSelected �� *
IsSelected+��  �@Q�
IsExpanded �� *
IsExpanded+ P�  �@Q�
IsSelected . . ���  � �� �#adc6e5��  � �� �#0575AF��  � ��$
 Bold"���  � ��$ 12(�]�  ��  �@Q�
IsSelected . ��  �@Q�IsSelectionActive .   ��%9   '��       �� " ��.���  �@��CornerRadius$ 2�� ��%�  (��       * SystemColors.HighlightBrushKey+)(�� V     *SystemColors.ControlBrushKey+)(�� �     *$"SystemColors.HighlightTextBrushKey+)(�� �     *" SystemColors.ControlTextBrushKey+)�� $	 0,1&�$	 0,0&�.��� $ 	#FFD9F4FF��$ 0Q�� $ 	#FF9BDDFB��$ 1Q��� $	 0,1&�$	 0,0&�.��� $ 	#FFEEEDED��$ 0Q�� $ 	#FFDDDDDD��$ 1Q��� $ Black���� $ Black���� -   $��txtErrorinConfig��$��10q�$	��Top=�$��Center=�$y�	Collapsed=��� $��1�� �� ResizeBehavior$ PreviousAndNext=����#D6D7D6$��2q� �� ResizeDirection$ Columns=�$��Stretch=�$��Stretch=�$
��Auto��$��2,0,2,0q��� $��2��$��1��.@��� -   $��ShowCurrentWorkbin�����#D6D7D6$��1q����� $��No Workbin Selected��$
��Bold"�.��� .+�� -   $��gridWorkbinExplorer��$��2q�
������ $
��Auto=� �� -     �Name$  	grMessage��$��0 ��� -   $  rowMainMenu��$��65 ��� $��* �.+�� $
��Auto��$
��Auto��$��0��
������� �� $��30 ��� -   $  grdErrorColumn��$��* ��� $��30 �.+��� $C��=/Agent.Interaction.Desktop;component/Images/Contact/Error.png��$��0��$��5q�$��15��$��15���� -   $��txtInteractionMessage��$��1��$��Center=�$��5q��� -   $��btnCloseError��$��Right=�$��2��$��20��$��20��  NormalImageButton#��[ $��Center=�$��4q�.���� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png��! � ToolTip! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Close Error��$��0��" �� Padding$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��Close the error messages.��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��� -   $��StandardEmailImage��$y�	Collapsed=�$
��Auto�� .  $��1��$
��Auto��$��3q� �� -   $  gcInteractionQueue��$��0 ��� -   $  	gcWorkbin��$
��Auto ��� -   $  
gcOpenMail��$
��Auto ��� -   $  gcReply��$
��Auto ��� -   $  
gcReplyAll��$
��Auto ��� -   $  gcPrint��$
��Auto ��� -   $  
gcMarkdone��$
��Auto ��� -   $  gcDelete��$
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+��� -   $��btnWorkbinMail��$��5q�$��Calibri)�$��45��$��1��#��[ $	��Top=�$��Stretch=�.���� # �� Orientation$# Vertical=�$��Center=�.+��� -   $��imgWorkbinMail��$J��D/Agent.Interaction.Desktop;component/Images/EMail/Email transfer.png��$��30��$��35���� -   $��TitleWorkbinMail��$��Transfer��$��Center=�$ �� TextAlignment$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -    $��WorkbinMailHeading��$��Transfer Mail��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� -!   $��WorkbinMailContent��$!��Transfer the selected mail.��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -"   $��btnOpenMail��$��5q�$��Calibri)�$��45��$��2��#��[ $	��Top=�$��Stretch=�.���� $# Vertical=�$��Center=�.+��� -#   $��imgOpen��$F��@/Agent.Interaction.Desktop;component/Images/EMail/Email Open.png��$��30��$��35���� -$   $��	TitleOpen��$
��Open��$��Center=�$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��	Open Mail��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� $"��Open selected mail in window��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -%   $��btnReplyMail��$��5q�$��Calibri)�$��45��$��3��#��[ $	��Top=�$��Stretch=�.���� $# Vertical=�$��Center=�.+��� -&   $��imgReply��$G��A/Agent.Interaction.Desktop;component/Images/EMail/Email Reply.png��$��30��$��35���� -'   $��
TitleReply��$��Reply��$��Center=�$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��
Reply Mail��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��Reply to selected mail.��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -(   $��btnReplyAllMail��$��5q�$��Calibri)�$��45��$��4��#��[ $	��Top=�$��Stretch=�.���� $# Vertical=�$��Center=�.+��� -)   $��imgReplyAll��$K��E/Agent.Interaction.Desktop;component/Images/EMail/Email Reply all.png��$��30��$��35���� -*   $��TitleReplyAll��$��	Reply All��$��Center=�$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��	Reply All��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� $%��Reply to all for selected mail.��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -+   $��btnPrint��$��5q�$��45��$��Calibri)�$��5��#��[ $	��Top=�$��Stretch=�.���� $# Vertical=�$��Center=�.+��� $A��;/Agent.Interaction.Desktop;component/Images/Email/Print.png��$��30��$��35���� $��Print��$��Center=�$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Print Interaction��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� $%��Print the selected interaction.��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -,   $��btnMarkDone��$��5q�$��45��$��Calibri)�$��6��#��[ $	��Top=�$��Stretch=�.���� $# Vertical=�$��Center=�.+��� --   $��imgMarkDone��$F��@/Agent.Interaction.Desktop;component/Images/EMail/Email Done.png��$��30��$��35���� -.   $��TitleMarkDone��$��	Mark Done��$��Center=�$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -/   $��MarkDoneMailHeading��$��	Mark Done��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� -0   $��MarkDoneMailContent��$��Mark done selected mail.��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -1   $��	btnDelete��$��5q�$��45��$��Calibri)�$��7��#��[ $	��Top=�$��Stretch=�.���� $# Vertical=�$��Center=�.+��� -2   $��	imgDelete��$H��B/Agent.Interaction.Desktop;component/Images/EMail/Email delete.png��$��30��$��35���� $��Delete��$��Center=�$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Delete Mail��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� -3   $��txtDeleteToolTip��$��Delete the selected mail.��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -4   $��btnSnapShot��$��45��$��0,0,8,0q�$��5q�$��Calibri)�$��9��#��[ $	��Top=�$��Stretch=�y��� *IQVisiblity+.���� $# Vertical=�$��Center=�.+��� -5   $��imgSnapShot��$<��6/Agent.Interaction.Desktop;component/Images/Reload.png��$��30��$��35���� -6   $��TitleSnapShot��$��Center=�$��Snapshot��$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -7   $��SnapShotHeading��$��Snapshot��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� -8   $��SnapShotContent��$&�� Refresh the list of interactions��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -9   $��	btnExpand��$��45��$��0,0,8,0q�$��5q�$��Calibri)�$��10��#��[ $	��Top=�$��Stretch=�.���� $# Vertical=�$��Center=�.+��� -:   $��	imgExpand��$A��;/Agent.Interaction.Desktop;component/Images/HideDetails.png��$��30��$��35���� -;   $��TitleExpand��$��Center=�$��Hide Details��$$ Center=�$��Calibri)�$
��Wrap=�$��2,0,2,0q�! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -<   $��ExpandMailHeading��$��Hide Details��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� -=   $��ExpandMailContent��$%��Hide details for selected mail.��$��1��$" 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� ->   $��svMainContent��$��2��$
��Auto=�   VerticalScrollBar��%�   '��       7  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8   .��� -?   $��Middle��$y�	Collapsed=�.+�� -@   $��
MiddleGrid��$��0��$��1�� �� -A   $  rowdefHistory��$
��Auto ��� -B   $  rowdefDetails��$	��300��$��* �.+�� -C   $��gridWorkbin��$��0��$��2q� % �� 
TargetType  ScrollBarLineButton& ]� DecreaseRepeatButton 	 ScrollBarPageButton' ]� Thumb 
 ScrollBarThumb  HorizontalNormalBrush  HorizontalNormalBorderBrush( ]� IncreaseRepeatButton  HorizontalScrollBar  NormalBrush  NormalBorderBrush��%  &	       7  7 	 7 
 7  7  7 	 7  &	 �    7  7 	 7 
 7  7  7 	 7  '�� j    7  7  �� "% ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����. & �� 8�� $��ScrollBar.PageUpCommand��' n� 8�� $��1,0,1,0q�8�� 8�� ( �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� "% ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.  & �� 8�� $��ScrollBar.PageLeftCommand��' n� 8�� $��0,1,0,1q�8�� 8�� ( �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8   P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  .+�)  #Microsoft.Windows.Controls.DataGrid  -D   $��	dgWorkbin��
�������$��Stretch=�$��Stretch=�)   SelectionUnit$) FullRow=�$
��Left=�*   HorizontalGridLinesBrush* �#D6D7D6+   VerticalGridLinesBrush+ �#D6D7D6,   CanUserAddRows, .  -   CanUserDeleteRows- .  .   AutoGenerateColumns. .  /   RowHeaderWidth$/ 0Q�$	��150������ *SelectedWorkbinDetails+0 � ContextMenu0 ��   ContextMenu#��[ 1   Columns1 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /  )Microsoft.Windows.Controls.DataGridColumn2   MaxWidth$2 30Q�3   MinWidth$3 30Q�4   Header$4  ��5   CellTemplate5 �� .R��� $# 
Horizontal=�.+��� $��20��$��20��$��2q����� *	MailImage+! �� $# 
Horizontal=�.+��� ���� *TypeId+�� $�� mail.��  6   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$6 * $
4 From��5 �� .R��� $# 
Horizontal=�.+��� $��4,4,0,4q����� *From+! �� $" 5q����� *From+  $6 * $4 Subject��5 �� .R��� $# 
Horizontal=�.+��� $��4,4,0,4q����� *	Subject+! �� $" 5q����� *	Subject+  $6 * $4 Received��5 �� .R��� $# 
Horizontal=�.+��� $��4,4,0,4q����� *ReceivedDate+! �� $" 5q����� *ReceivedDate+   RowMenu7 � DataContext8 �� RelativeSource9 m� Icon��%�  &	       7  �� -E   8��  7 �� *PlacementTarget.DataContext+8 �� *Self+.@�m� $��mnuMailOpen��-F   $
��Open��9 �� $��mnuOpenimage��$F��@/Agent.Interaction.Desktop;component/Images/EMail/Email Open.png��$��15��$��15��m� $��mnuMarkDone��-G   $��	Mark Done��9 �� $��mnuMarkDoneimage��$F��@/Agent.Interaction.Desktop;component/Images/EMail/Email Done.png��$��15��$��15��m� $��mnuReply��-H   $��Reply��9 �� $��mnureplyimage��$G��A/Agent.Interaction.Desktop;component/Images/EMail/Email Reply.png��$��15��$��15��m� $��mnuReplyAll��-I   $��	Reply All��9 �� $��mnuReplyAllimage��$K��E/Agent.Interaction.Desktop;component/Images/EMail/Email Reply all.png��$��15��$��15��m� $��	mnuDelete��-J   $��Delete��9 �� $H��B/Agent.Interaction.Desktop;component/Images/EMail/Email delete.png��$��15��$��15��m� $��mnuMove��$��Move to��9 �� $E��?/Agent.Interaction.Desktop;component/Images/Email/transfer1.png��$��15��$��15��:   RowStyle: �� ,  &Microsoft.Windows.Controls.DataGridRow"  .���  �@�ContextMenu# [  P�  �@ 
IsSelected . . ���  � ��  HighlightedBrush# [ P�  �@J�IsMouseOver . . ���  � �� �#E3F9FE� -K   $��grdIQ��$
��Auto��y��� *IQVisiblity+ �� $	��150���� $��30 �.+��� -L   $��brdIQ��$��2q�$��0�����#D6D7D6$��1q�$
��Auto��y��� *IQVisiblity+._�  -M   $
��dgIQ��
�������$��Stretch=�$��Stretch=�$	��150��$
��Left=�* �#D6D7D6+ �#D6D7D6. . , .  $/ 0Q�- .  ;   
IsReadOnly; . <   CanUserSortColumns< .  $��2q�$
��Auto=�=   	RowHeight$= 25Q����� y��� *IQVisiblity+: �� "   P�  �@ 
IsSelected . . ���  � ��# [ P�  �@J�IsMouseOver . . ���  � �� �#E3F9FE�� -N   $��	brdPaging��$��1�����#D6D7D6$��2q�$��2,2,2,2q�> �� Padding$> 2q�
������ y��� *IQVisiblity+._�� .+��� $��2,0,0,0q�$��1��$# 
Horizontal=�$
��Left=�.+��� -O   $��btnFirstPage��#��[ $��20��.���� $��10��$��10��$J��D/Agent.Interaction.Desktop;component/Images/Contact/FirstPage-01.png��! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -P   $��FirstHeading��$��
First Page��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� -Q   $��btnPreviousPage��#��[ $��20��.���� $��10��$��10��$M��G/Agent.Interaction.Desktop;component/Images/Contact/PreviousPage-01.png��! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -R   $��PreviousHeading��$��Previous Page��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� $# 
Horizontal=�$��Center=�.+��� -S   $��txtStart��$��0��$��Center=��� $	�� - ��$��Center=��� -T   $��txtEnd��$��0��$��Center=�$��5,0,0,0q��� -U   $��btnNextPage��#��[ $��20��.���� $��10��$��10��$I��C/Agent.Interaction.Desktop;component/Images/Contact/NextPage-01.png��! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -V   $��NextHeading��$��	Next Page��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� -W   #��[ $��0��$y�	Collapsed=�.���� -X   ? ��Name$? imgLastIndex��$��10��$��10��$I��C/Agent.Interaction.Desktop;component/Images/Contact/LastPage-01.png��! �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -Y   $��LastHeading��$��	Last Page��$��0��$" 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��0,0,2,0q�$��1��$��Center=�$# 
Horizontal=�$��Right=�.+��� $��Items per page ��$��Center=�$��5,0,0,0q��� -Z   $��cmbItemsPerPage��@ �� SelectedIndex$@ 0��$y�Visible=�$��Center=�A �� Text$A  ��$��5��  ComboBox#��[ $��5,0,0,0q����� *AvailableDirPageSizes+�� -[   $��txtNoDetails��$��2��$	��Top=�$��0,50,0,0q�$��Center=�$y�Visible=�$/��)Please select interaction to view details���� -\   $��txtNocontactServer��$��2��$	��Top=�$��0,50,0,0q�$��Center=�$y�Visible=�$*��$The Contact Server is out of service���� -]   $��
tbCallBack��$��2�����#D6D7D6$��1q�$��2q�.@��� $��Details��
������ .��� .+�� -^   $��gridDetails��$��2q�$y�	Collapsed=� �� $��* � �� $
��Auto ��� $
��Auto ��� -_   $  dockCC��$��20 ��� -`   $  dockBCC��$��20 ��� -a   $  	dockState��$
��Auto ��� -b   $  
dockAttach��$
��Auto ��� $��* �.+�]� $��0��$��0��$��1q�.+�� -c   $��subjectGrid��$	��Top=� �� �� $��20 � �� .+��� -d   $��txtSubjectBold��B �� 	UndoLimit$B 10��$��0,0,5,0q�$�� 0��$��0��$
��Left=�
������ $��0q�$��Center=�C �� 
IsReadOnlyC . $
��Bold"�D � Cursor$
D Hand������ *	Subject+.��! �� $" 5q����� *	Subject+.���� -e   $��txtDot��$��1��$��Center=�
�������$	��...��$
��Bold"�$��Right=�]� -f   $��ToFrom��$��1��$��0��$��1q����� E j� Stretch$
E None=�F j� TileMode$
F Tile=�G j� Viewport$G 
0,0,741,11�H j� ViewportUnits$H Absolute=�.+��  �� $
��Auto ��� $��* ��� $
��Auto � �� -g   $
  gcTo��$
��Auto ��� -h   $  gcFrom��$
��Auto �.+��� -i   $��lblTo��$��Calibri)�$��Center=�$��0��$
��Bold"�$	��To:���� -j   $��txtTo��$B 10��$��2��$��Calibri)�$��1��$��Center=�
������ $��0q�C . $
D Hand������ *To+.��! �� $" 5q����� *To+.���� -k   $��lblFrom��$��Calibri)�$��1��$��0��$��Center=�$
��Bold"�$��From:���� -l   $��txtFrom��$B 10��$��Calibri)�$��1��$��1��$��Center=�
������ $��0q�C . $
D Hand������ *From+.��! �� $" 5q����� *From+.���� -m   $B 10��$��1��$��2��
������ $��0q�C . $
D Hand������ *ReceivedDate+.��! �� $" 5q����� *ReceivedDate+.��]� -n   $��dkpCc��$��0��$��2��$��1q�.+��  �� $��40 ��� $��* � �� $
��Auto �.+��� -o   $��lblCc��$��Calibri)�$��Center=�$��0��$��0��$
��Bold"�$	��Cc:���� -p   $��	txtCcText��$B 10��$��Calibri)�$��0��$��1��$��Center=�
������ $��0q�C . $
D Hand������ *CC+.��! �� $" 5q����� *CC+.��]� -q   $��dkpBCc��$��0��$��3��$��1q�.+��  �� $��40 ��� $��* � �� $
��Auto �.+��� -r   $��lblBCc��$��Calibri)�$��Center=�$��0��$��0��$
��Bold"�$
��BCc:���� -s   $��
txtBCcText��$B 10��$��Calibri)�$��0��$��1��$��Center=�
������ $��0q�C . $
D Hand������ *BCC+.��! �� $" 5q����� *BCC+.��]� -t   $��dkpState��$��0��$��4��$��1q�.+��  �� $��40 ��� $��* � �� $
��Auto �.+��� -u   $��lblState��$��Calibri)�$��Center=�$��0��$��0��$
��Bold"�$��State:���� -v   $��txtStateText��$B 10��$��Calibri)�$��0��$��1��$��Center=�
������ $��0q�C . $
D Hand������ *InterationState+.��! �� $" 5q����� *InterationState+.���� -w   $��attacScroll��$��5��$��0��$
��Auto��$��35��$��Stretch=���. $��Hidden=�$
��Auto=�$��0,0,2,0q�.��� -x   $��dpAttachments��$
��Auto=�]� -y   $��dcMailMessageContainer��$��6��$	��100��� -z   $��gridOtherMediaDetails��$��2q�$y�	Collapsed=�.+�]� $��5q�.+��  �� $��40 ��� $��* � �� $
��Auto �.+��� $��Calibri)�$��Center=�$��0��$��0��$
��Bold"�$��State:���� -{   $B 10��$��Calibri)�$��0��$��1��$��Center=�
������ $��0q�C . $
D Hand������ *InterationState+.��! �� $" 5q����� *InterationState+.���� $
��Note��
�������.���� -|   $��svNote��$��2q�$
��Auto=�$
��Auto=�.���� -}   $��txtNotes��$B 10��$��0��$��0��$
��Auto=�C . $
��Auto=�I �� AcceptsReturnI . ���� *
EmailNotes+�� -~   $��tapCaseInfo��$��	Case Data��
�������.���� $��2q�$��Disabled=�$
��Auto=�.��]� -   $��dockCaseData��$y�	Collapsed=���. $	��Top=�$
��Left=�.+�� 
�������$��1��$��2q� �� $
��Auto ��� $
��Auto �.+��� -�   $��bdrUSerDataContainer�����#D6D7D6$��0��$��1q�$��2q�$��0��$��Stretch=�._�� .+�  -�   $��
dgUserData��
�������$��Stretch=�$��Stretch=�$
��Left=�* �#D6D7D6+ �#D6D7D6. .  $J   HorizontalScrollBarVisibility$J Disabled=�, .  $/ 0Q�- .  < . ���� *
CaseData+1   $6 * $	4 Key��5 �� .R��� $# 
Horizontal=�.+��� $��4,4,0,4q����� *Key+  $6 * $4 Value��5 �� .R��� $# 
Horizontal=�.+��� $��4,4,0,4q����� *Value+�� -�   $��txtNoCaseData��$'��!No case data available to display��$��Center=�$��20q�]� 
�������$��1��$	��Top=�$��2q�.+��� -�   $��lblDispositionHeader��$	��Top=�$��Calibri)�$��12(�$��Bottom=�$
��Bold"�$��Disposition Code:�����	#E643AFC6�� -�   $��lblDisposition��$
��Left=�$��Calibri)�$��12(�$
��Bold"�$��DispositionCode:���� -�   $��txtDisposition��$
��Left=�$��Center=�$��Calibri)�$��12(����� *DispositionKey+�� -�   $��
txtLoading��$��	Loading..��$��12(�$��Center=�$��20q��� -�   $��	txtNoItem��$	��Top=�$��Center=�$��20,40q�$y�	Collapsed=�$*��$No interaction available to display.���� -�   $��txtUnavailableError��$y�	Collapsed=�$$ Center=�$" 20q�!Work bin temporaily unavailable