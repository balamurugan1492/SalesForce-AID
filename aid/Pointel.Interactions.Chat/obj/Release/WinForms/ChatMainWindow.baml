   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  T PPointel.Interactions.Chat, Version=5.0.3.5, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Pointel.Interactions.Chat.CustomControls(Pointel.Interactions.Chat.CustomControls [2clr-namespace:Pointel.Interactions.Chat.Converters$Pointel.Interactions.Chat.Converters U/clr-namespace:Pointel.Interactions.Chat.Helpers!Pointel.Interactions.Chat.Helpers 7   1Pointel.Interactions.Chat.WinForms.ChatMainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     6sys&clr-namespace:System;assembly=mscorlib     6my-http://schemas.microsoft.com/wpf/2008/toolkit  JCustomControl6clr-namespace:Pointel.Interactions.Chat.CustomControls  @convert2clr-namespace:Pointel.Interactions.Chat.Converters  ;local/clr-namespace:Pointel.Interactions.Chat.Helpers  R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  -   $��
ChatWindow��  � AllowsTransparency  . $
��Auto��$
��Auto��$	��490�� � WindowStyle$
 None=� � 
ResizeMode$ 	CanResize=�
������  � WindowStartupLocation$ Manual=� � Icon$E ?/Agent.Interaction.Desktop;component/Images/Chattaskbaricon.ico�� � Topmost .   � Title �� *ChatFromPersonName+ � 	Resources  �� MergedDictionaries	 �� Source   	WavyBrush
 j� Viewbox j� ViewboxUnits j� Viewport j� ViewportUnits j� TileMode !� Visual J� SnapsToDevicePixelsG  APointel.Interactions.Chat.Converters.StringtoImageSourceConverter  StringToImageSourceConvert��  �� $R	 L/Pointel.Interactions.Chat;component/Resources/StylesResourceDictionary.xaml?�%�   &	        &	 �     !� $
 0,0,3,3�$ Absolute=�$ 	0,-5,7,15�$ Absolute=�$
 Tile=� F� -��������> �@B     �� ��  �0  5 	���	#FFFF0000$	��0.2�� .   .��� -   $��MainGrid��.+��� -   $��
MainBorder��$
��0.89q�$��8q�
����������� *MainBorderBrush+._��  � RowDefinitions �� $
��Auto ��� $��* �.+��� -   $��RightSideRect��$��2��
������ $��3�� K� ZIndex$
 1000��$��Right=�$��Stretch=��� -   $��RightbottomSideRect��$��1��
������ $��8��$��8��$
 2200��$��Right=�$��Bottom=��� -   $��BottomSideRect��$��1��
������ $��3��$
 1000��$��Stretch=�$��Bottom=�� $��0�� � ColumnDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� -   $��agentStateBorder��$��0��$��5,0,0,0q�
�������$��60��._���  �� Orientation$ 
Horizontal=�.+��� -   $��imgChatAgentInfo��$��16��$��Center=�$?��9/Agent.Interaction.Desktop;component/Images/Chat/Chat.png��$��1q��� -	   $��1��$��18�����#007EDF$	��Top=�$��0,-1,0,0q�._��  �� -
    �Name$ grdTitle��$��* ��� $
��Auto �.+��� -   $��lblTitleStatus��$��0��$��Stretch=�$��Calibri)�$��14(�
�������$
��Bold"�$��Stretch=�$��5,0,10,0q�$��CharacterEllipsis=����� *TitleBgColor+���� *ChatWindowTitleText+����  �� ElementName$ grdTitle�� �� Path$ Width��� -   $��txtChatStatus��$��1��$��Right=�$��5,0,5,0q�$��Calibri)�$��14(�
�������$
��Bold"����� *ChatTypeStatus+�� $��2��$��60��$
��Auto��$��Right=�$	��Top=�$ 
Horizontal=�.+��� -   $��btnMinimize��$��16��$��20��  MinimizeButton#��[ �� -   $��btnMaximize��$��16��$��20��  maximizeButton#��[ �� -   $��btnExit��$��16��$��20��  CloseButton#��[ � -   $��grdMain��
�������$��1�� �� $��* �$	��440�� �� -   $ 
columnChat��$��* �$	��400���� -   $ columnButtons��$��19 ��� -   $ columnHistory��$	��400��.+�� -   $��grdChatMain��$��0�� �� $��* �.+��� -   $��tabChat��$��2q�
�������$��Stretch=�$	��450��.@��� -   $��tabItemCustomer����� $��25��$
��Auto��$��Center=�$��0,2,0,0q� �� -   $ tabHeaderRow��$��* ��� $
��Auto ��� $
��Auto �.+��� -   $��txtblkTabItemCustomer��$��0��$��5,0,10,0q�$��CharacterEllipsis=�$��Center=�$��Stretch=����#0071C6$
��Bold"����� *ChatFromPersonName+���� $ tabHeaderRow��$ Width��� $��20��$��20��$��2,2,0,2q�$��1������  �� ImageSource$G A/Agent.Interaction.Desktop;component/Images/Chat/Chat.Disable.png��.+��� -   $��imgChatIcon��$��20��$��20��$��Center=�$��0,0,0,2q�"  WpfAnimatedGif.ImageBehavior   AnimatedSource �� *NotificationImageSource+9  3Pointel.Interactions.Chat.CustomControls.TimerLabel  -     Name$ lblTabItemShowTimer��$��2��   Text$ 
[00:00:00]��$
��Left=�$��Center=�$��61��$��5,3,3,3q����#0071C6$
��Bold"�.��� 
������ $��2,4,2,2q� �� -   $ grdrow_error������ *ErrorRowHeight+�� -   $ ChatToolMenu������ *ChatToolBarRowHeight+�� $��* �.+��� $��0�����#FFA87D$��1q�._�� $
��Auto��$
��Auto�����#FFFFCB �� $��30 ��� -   $ grdErrorColumn��$��* ��� $��30 �.+��� $��5,2,0,2q�$��0��$��Center=�$P��J/Agent.Interaction.Desktop;component/Images/Chat/Chat.Session.Expiered.png��$��20��$��20���� -   $��lblInformation��$��1��$��4q�$��Stretch=�$
��Wrap=�$
��Bold"����� *ErrorMessage+�� -   $��btnCloseError��$��Right=�$��2��$��15��$��15��  NormalImageButton#��[ $��Center=�$��4q�.���� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png��� $��1��$��2q� �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $	��50* ��� $��* �.+��� -    $��btnChatRelease��$��0��$��45��$��55��$��55��$��5,0,0,0q�#��[ $	��Top=�$��Stretch=�{��� *IsEnableRelease+.���� $ Vertical=�$��Center=�.+��� $��40��$��40������ *ReleaseImageSource+�� $��Center=� �� TextAlignment$ Center=�$��Release��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q�{��� *IsEnableRelease+ � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -!   $��ReleaseHeading��$��End Chat��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -"   $��ReleaseContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Release the Chat.���� -#   $��btnChatTransfer��$��1��$��45��$��55��$��55��$��5,0,0,0q�#��[ $	��Top=�$��Stretch=�{��� *IsEnableTransfer+.���� $ Vertical=�$��Center=�.+��� $��40��$��40������ *TransImageSource+�� $��Center=�$ Center=�$��Transfer��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q�{��� *IsEnableTransfer+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -$   $��TransferHeading��$��Chat Transfer��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -%   $��TransferContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$8��2Agent can make the transfer chat to another agent.���� -&   $��btnChatConference��$��2��$��55��$��65��$��55��$��5,0,0,0q�#��[ $	��Top=�$��Stretch=�{��� *IsEnableConference+.���� $ Vertical=�$��Center=�.+��� $��40��$��40������ *ConfImageSource+�� $��Center=�$ Center=�$��
Conference��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q�{��� *IsEnableConference+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -'   $��ConferenceHeading��$��Chat Conference��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -(   $��ConferenceContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$:��4Agent can make the conference chat to another agent.���� -)   $��btnChatConsult��$��3��$��45��$��55��$��55��$��5,0,0,0q�#��[ $	��Top=�$��Stretch=�{��� *IsEnableChatConsult+.���� $ Vertical=�$��Center=�.+��� $��40��$��40������ *ConsultChatImageSource+�� $��Center=�$ Center=�$��Consult��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q�{��� *IsEnableChatConsult+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -*   $��ConsultHeading��$��Chat Consult��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -+   $��ConsultContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$7��1Agent can make the Consult chat to another agent.���� -,   $��btnChatDone��$��4��$��45��$��55��$��55��$��5,0,0,0q�#��[ $	��Top=�$��Stretch=�{��� *IsEnableDone+.���� $ Vertical=�$��Center=�.+��� $��40��$��40������ *DoneImageSource+�� $��Center=�$ Center=�$
��Done��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q�{��� *IsEnableDone+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� --   $��DoneHeading��$��	Mark Done��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -.   $��DoneContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$=��7Agent can mark done after release the chat interaction.���� -/   $��btnVoiceConsultation��$��5��$��45��$��55��$��55��$��5,0,0,0q�#��[ $	��Top=�$��Stretch=�{��� *IsEnableVoiceConsult+.���� $ Vertical=�$��Center=�.+��� $��40��$��40������ *VoiceConsultImageSource+�� $��Center=�$ Center=�$��Voice��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q�{��� *IsEnableVoiceConsult+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -0   $��VoiceConsultHeading��$��Voice Consult��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -1   $��VoiceConsultContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$1��+Agent Make a Consult call to another agent.��� -2   $��grdMainChatArea��$��2��$��Stretch=� �� -3   $ grdRowStatus������ *ChatAgentsStatusRowHight+�� $��* �.+��� -4   $��1q����#D6D7D6$��2q�$��0��._��  �� -5   $ gdChatPersons��$��* ��� $
��Auto ��� $
��Auto � �� $��30 ��� $
��Auto �.+�)  #Microsoft.Windows.Controls.DataGrid  -6   $��DGChatPersonsStatus��$��2��$��Stretch=�   HeadersVisibility$
 None=�    RowHeaderWidth$  0Q�$��2q�
�������$��Stretch=�$
��Left=�!   HorizontalGridLinesBrush
! ���� "   VerticalGridLinesBrush
" ���� #   AutoGenerateColumns# .  $   CanUserAddRows$ .  
������ %   CanUserDeleteRows% .  &   CanUserSortColumns& .  $��0������ *ChatPersonsStatusInfo+���� $ gdChatPersons��$ Width�'   Columns' 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn(   
Visibility$( Hidden=�4  .Microsoft.Windows.Controls.DataGridBoundColumn)   Binding) �� *	PlaceID+7  1Microsoft.Windows.Controls.DataGridTemplateColumn  *   Width8	  2Microsoft.Windows.Controls.DataGridLengthConverter$* 25	 +   MinWidth$+ 25Q�,   MaxWidth$, 25Q�-   CellTemplate- �� .R��� $��btnChatPersonDetails��-7   $��15��#��[ . � Cursor$
. Hand��$��0q�.���� $;��5/Agent.Interaction.Desktop;component/Images/Arrow.png��$
��Fill=�$��10��$��10��$��Bottom=�  $* *	 $	+ 110Q�- �� .R��� $��lblChatPersonName��$��Center=�$��CharacterEllipsis=����� *ChatPersonName+  $* 25	 $+ 25Q�$, 25Q�- �� .R��� $��imgChatStatusIcon��$��18��$��18������ *ChatPersonStatusIcon+  $	* 100	 $+ 90Q�$	, 120Q�- �� .R��� $��lblChatStatus��$��Center=����� *ChatPersonStatus+�� $��2q�$��1��$��1q�$��0��$��1��$	��Top=�y��� *InProgressIXNCountVisibility+._��� $ 
Horizontal=�$��Center=�y��� *InProgressIXNCountVisibility+.+��� -8   $��imgInteractionsStatus��$
��Left=�$Q��K/Agent.Interaction.Desktop;component/Images/Chat/Chat.Interaction.Count.png��$
��Left=�$��LeftToRight=�$��Center=�$��18��$��18��$��2q��� -9   $��lblInteractionCount��$
��Left=�$��Center=����� *InprogressInteractionCount+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -:   $��IXNCountHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *TotalInprogessIXNCount+�� -;   $��stkPanelIXNCountContent��$��
10,5,10,10q�$ Vertical=�$��1��$
��Auto��$	��200������ / �� 
TargetType"/ ��0 �� Triggers0 P� 1 P� Property1 �@J�IsMouseOver2 P� Value2 . . ��� 3 �� Property3 � ��4 �� Value4 �#939589�� 3 � ��4 �#CEE7F7�� -<   $��borderRecentIXN��$��2,2,5,2q�$��2��$��1q�$��0��$��1��$	��Top=�y��� *RecentIXNListVisibility+._��� $ 
Horizontal=�$��Center=�y��� *RecentIXNListVisibility+.+��� -=   $��imgInteractionsList��$
��Left=�$E��?/Agent.Interaction.Desktop;component/Images/Chat/RecentList.png��$
��Left=�$��LeftToRight=�$��Center=�$��18��$��18��$��2q��� ->   $��lblInteractionList��$
��Left=�$��Center=����� *RecentIXNCount+���� "/ ��0 P� 1 �@J�IsMouseOver2 . . ��� 3 � ��4 �#939589�� 3 � ��4 �#CEE7F7�� -?   $��ScrollViewer��$��1��$
��Auto=���.     HorizontalScrollBar  VerticalScrollBar��%M  '��       7  7  �� "/ ��.��� 3 �@J�SnapsToDevicePixels4 . �� 3 �@�OverridesDefaultStyle4 . 0 P� 1 �@��Orientation$2 
Horizontal=�. ��� 3 � ��$
4 Auto���� 3 � ��$4 18���� 3 � ��84   P� 1 �@��Orientation$2 Vertical=�. ��� 3 � ��$4 18���� 3 � ��$
4 Auto���� 3 � ��84  .���  �� $��* ��� -@   $ grdRowSendMainChat������ *SendChatWindowRowHeight+�� -A   $ grdRowConsultChat������ *ConsultChatWindowRowHeight+.+�B
  <Pointel.Interactions.Chat.CustomControls.BindableRichTextBox
  -B   5 
 Name$5 mainRTB��B  <Pointel.Interactions.Chat.CustomControls.BindableRichTextBox"6   PreviewMouseRightButtonDown$)6 #mainRTB_PreviewMouseRightButtonDown��7   TextChanged$7 mainRTB_TextChanged��$��0��$��Stretch=�$	��150��$	��280��8 �� 
IsReadOnly8 . $��2q�9 �� IsDocumentEnabled9 . $
��Auto=�: J� IsManipulationEnabled: .  ; J� 	AllowDrop; . $��1q����#D6D7D6< 
  	Documents< �� *RTBDocument+{��� *IsConversationRTBEnabled+ = �� RelativeSource��%h  '��       'J� O     '�� x     7  �� "/ ��.��� 3 �@�ToolTip4 �� $ NavigateUri�= �� *Self+�� "/ J�.��� 3 �@��Margin$4 2q��� "/ ��.��� 3 �@J�SnapsToDevicePixels4 . �� 3 �@�OverridesDefaultStyle4 . 0 P� 1 �@��Orientation$2 Vertical=�. ��� 3 � ��$4 18���� 3 � ��$
4 Auto���� 3 � ��84   > � ContextMenu> �� ]� -C   $��dckSendChatWindow��$
��Auto��$��1��$��Bottom=�.+��� $��1q����#D6D7D6$��2q�._�� $��75��$	��Top=� �� $	��10* ��� $
��Auto � �� $��* ��� ���� *ChatContentRowHeight+.+��� -D   $��txtChatSend��$��0,0,0,1q�
�������$��0��? �� AcceptsReturn? . $��0��$
��Auto��$��Stretch=�$@ �� HorizontalScrollBarVisibility$
@ Auto=�"A �� VerticalScrollBarVisibility$
A Auto=�  controltempTextbox#��[ $��2q�> Z� {��� *IsTextMessageEnabled+ ��%�  '��       7  'J� �     &	  �     �� "/ ��.��� 3 �@J�SnapsToDevicePixels4 . �� 3 �@�OverridesDefaultStyle4 . 0 P� 1 �@��Orientation$2 Vertical=�. ��� 3 � ��$4 18���� 3 � ��$
4 Auto���� 3 � ��84   �� "/ J�.��� 3 �@��Margin$4 1q�!� $
 0,0,3,3�$ Absolute=�$ 	0,-1,7,15�$ Absolute=�$
 Tile=� F� -��������> �@B     �� ��  �0  5 	
���  ��$	��0.2�� . �� $��1��$��0��$��1q����#D6D7D6$��1q�$	��Top=�._�� -E   $��grdCheckURL�� �� $	��10* ��� $
��Auto �.+��� $��0��$��1q����#D6D7D6$��0q�$	��Top=�._��  �� -F   $��* ��� $
��Auto �.+��� -G   $��
txtPushURL��B �� 	UndoLimit$B 10��$��29��$��0��$��0q� 	 MyWaterMarkStyle#��[	 C �� TextWrapping$
C Wrap=�
D � Tag$D Type URL to Push��{��� *IsTextURLEnabled+�� -H   $��btnCheckURL��$��1��$
. Hand��$��Center=�$
��Auto��#��[ $��25��$��Right=�{��� *IsButtonCheckURL+.���� $I��C/Agent.Interaction.Desktop;component/Images/Chat/Chat.Check.URL.png��$��20��$��20�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -I   $��CheckURLHeading��$��	Check URL��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -J   $��CheckURLContent��$��Check the URL in browser.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -K   $��btnAvailableURL��$��15��#��[ $
. Hand��$��0q�$��2,0,0,0q�$��1��{��� *IsButtonAvailableURL+.���� $;��5/Agent.Interaction.Desktop;component/Images/Arrow.png��$
��Fill=�$��10��$��10��$��Bottom=� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -L   $��AvailableURLHeading��$
��URLs��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -M   $��AvailableURL��$'��!Available URLs configured in CME.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -N   $��stkSendPanel��$
��Auto��$��1��$��2��.+��� -O   $��btnSendMessage��$��Stretch=�$��55��$��55��$��2,2,2,1q�$��Right=� 
 ChatSendButton#��[
 $
��Send��$��Normal"�{��� *IsButtonSendEnabled+�� -P   $��
btnSendUrl��$��Stretch=�$��15��$��55��$��2,0,2,2q�$��Right=�  NormalButton#��[ $��Normal"�{��� *IsButtonPushURLExpander+.���� $
��Fill=�$��10��$��10��$��Center=�$��0,2,2,2q����� *BtnArrowImageSource+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -Q   $��PushURLHeading��$��Push URL��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -R   $��PushURLContent��$��Show the Push URL area.��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��]� -S   $��dpConsultWindow��$��2��$��Stretch=�
������ � $��1��$��2,0,2,0q� �� $
��Auto ��� .+��� -T   $��btnShowHideIXNPanel��$��0��$��0��$
��Auto��#��[ $	��Top=�$	��Top=�.���� $ Vertical=�$��Center=�.+��� -U   $��imgShowHideIXNPanel��$?��9/Agent.Interaction.Desktop;component/Images/Hide_Left.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -V   $��ToolHeading��$
��Hide��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -W   $��ToolContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$*��$Agent can hide the interaction panel��A� E A� Rows$E 3��F A� Columns$F 1��$��1��$��0��.+�d� -X   $��btnData��$
. Hand��  
SideButton#��[ $��0,0,0,1q�.��� .+��� -Y   $��imgData��$��18��$��10��$��Right=����� *CasedataImageSource+�� $
��DATA��$��9(�$
��Wrap=�
�������d� -Z   $��btnContacts��$
. Hand��#��[ $��0,0,0,1q�.��� .+��� -[   $��imgContacts��$��18��$��10��$��Right=����� *ContactImageSource+�� $��CONTACT��$��9(�$
��Wrap=�
�������d� -\   $��btnResponses��$
. Hand��#��[ $��0,0,0,1q�.��� .+��� -]   $��imgResponse��$��18��$��10������ *ResponseImageSource+�� $��	RESPONSES��$��9(�$
��Wrap=�
�������� -^   $��grdUC��$��2q�$��2��$��Stretch=�� -_   $��popupRecentIXNDetails��$��Bottom=�G � 	StaysOpenG .  H � AllowsTransparencyH . $��Bottom=�.���� ���#ADAAAD$��1q�$��2q�$
��Auto��$
��Auto��
�������._��  �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -`   $��lblRecentIXNCount��$��2��$��0��$
��Left=�$��5q����� *RecentInteractionCount+  -a   $��DGRecentInteraction��$  0Q�
������ $��5q�
�������$
��Left=�$��Center=�$
��Left=�
! ���� 
" ���� # .  $ .  % .  $��2��$��1��$
 None=����� *RecentInteraction+'   I   Header$
I Icon��$
* Auto	 $+ 30Q�- �� .R��� $��
imgIXNIcon��$
��Left=�$��2,0,2,0q�$��15��$��15������ $ 	MediaType�J �� 	Converter#J [   $I Started Date��$
* Auto	 - �� .R��� $��txtIXNStartDate��$
��Left=�$��2,0,2,0q����� *InteractionStarted+  $I Subject��$
* Auto	 - �� .R��� $��txtIXNSubject��$
��Left=�$��2,0,2,0q����� *InteractionSubject+K   RowStyleK �� ,  &Microsoft.Windows.Controls.DataGridRow"/  0 P� 1 �@ 
IsSelected2 . . ��� 3 � ��  HighlightedBrush#4 [ �� -b   $��lblRemainingIXN��$��2��$��2��$
��Left=�$��5q����� *RemainingDetails+� -c   $��popupAgentInfo��$��Bottom=�G .  H . $��Bottom=�.���� ���#ADAAAD$��1q�$��10q�$
��Auto��$
��Auto��
�������._��  �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto �.+��� $ 
Horizontal=�$��2��$��0��$
��Left=�$��5q�.+��� -d   $��lblagentUserName��$��Italic$�$��User Name: ��$��5,0,0,0q��� -e   $��lblagentName��$��Normal$�$��5,0,0,0q����� *
UserName+�� $ 
Horizontal=�$��2��$��1��$
��Left=�$��5q�.+��� -f   $��lblagentPlace��$��Italic$�$��Using Place: ��$��5,0,0,0q��� -g   $��lblPlace��$��Normal$�$��5,0,0,0q����� *	PlaceID+