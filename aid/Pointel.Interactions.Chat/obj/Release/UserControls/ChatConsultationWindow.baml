   M S B A M L   `   `   `  ���� T  PPointel.Interactions.Chat, Version=5.0.3.5, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Pointel.Interactions.Chat.CustomControls(Pointel.Interactions.Chat.CustomControls  C    =Pointel.Interactions.Chat.UserControls.ChatConsultationWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     PCustomControl6clr-namespace:Pointel.Interactions.Chat.CustomControls     6my-http://schemas.microsoft.com/wpf/2008/toolkit  I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  :d2http://schemas.microsoft.com/expression/blend/2008  -   $
��Auto��  � 	Resources   �� MergedDictionaries �� Source   	WavyBrush j� Viewbox j� ViewboxUnits j� Viewport j� ViewportUnits j� TileMode !� Visual	 J� SnapsToDevicePixels��  �� $R L/Pointel.Interactions.Chat;component/Resources/StylesResourceDictionary.xaml?�%�   &	        !� $ 0,0,3,3�$ Absolute=�$ 	0,-5,7,15�$ Absolute=�$
 Tile=� F� -��������> �@B     �� ��  �0  5 	���	#FFFF0000$	��0.2��	 . .��� .+��� $��1q����#D6D7D6$��2q�._�� 
 � RowDefinitions
 �� $��30 ��� ���� *ConsultChatRowHight+.+�� $��0�����#D8E5FB � ColumnDefinitions �� $��* ��� $��30 �.+���  �� Orientation$ 
Horizontal=�$��25��$
��Auto��$��Center=�$��0,2,0,0q�$��0��.+��� -   $��lblTabItemCustomer��$��Center=�$��Center=����#0071C6$
��Bold"����� *ChatConsultWindowTitleText+�� -   $��imgChatIcon��$��20��$��20��$��Center=�$��2,0,0,2q�$?��9/Agent.Interaction.Desktop;component/Images/Chat/Chat.png��9   3Pointel.Interactions.Chat.CustomControls.TimerLabel  -     Name$ lblTabItemShowTimer��   Text$ 
[00:00:00]��$
��Left=�$��Center=�$��61��$��5,3,3,3q����#0071C6$
��Bold"��� -   $��btnShowHideConsultEx��$��18��$��1��$��18�� K� ZIndex$
 1000��$��0,5,5,0q�  numberButton1#��[ $	��Top=�$��Right=�$��Stretch=�.���� -   $��imgExpander��$��15��$��15������ *ImgConsultChatExpander+� 
������ $��2,4,2,2q�$��1�� �� $��* �
 �� -    �Name$ ChatToolMenu��$
��Auto ��� $��* �.+�� $��0��$��2q� �� $
��Auto �.+��� -   $��btnChatRelease��$��0��$��45��$��55��$��55��$��5,0,0,0q�  NormalImageButton#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $��40��$��40������ *ConsultReleaseImageSource+�� $��Center=� �� TextAlignment$ Center=�$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q����� *ConsultReleaseText+{��� *IsEnableRelease+ � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 
 �� $
��Auto ��� $��* �.+��� -	   $��ReleaseHeading��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *ConsultReleaseTTHeading+�� -
   $��ReleaseContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200������ *ConsultReleaseTTContent+� $��1��$��Stretch=�
 �� -   $ grdRowPersonStatus��$
��Auto ��� $��* �.+��� -   $��brdPersonsStatus��$��1q����#D6D7D6$��2q�$��0��._�)  #Microsoft.Windows.Controls.DataGrid  -   $��DGChatPersonsStatus��$��Stretch=�   HeadersVisibility$
 None=�   RowHeaderWidth$ 0Q�$��2q�
�������$��Stretch=�$
��Left=�   HorizontalGridLinesBrush
 ����    VerticalGridLinesBrush
 ����    AutoGenerateColumns .     CanUserAddRows .  
������    CanUserDeleteRows .     CanUserSortColumns .  ���� *ChatConsultPersonStatusInfo+   Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   
Visibility$ Hidden=�4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *	PlaceID+7  1Microsoft.Windows.Controls.DataGridTemplateColumn     Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$ 25     MinWidth$  25Q�!   MaxWidth$! 25Q�"   CellTemplate" �� .R��� $��btnChatPersonDetails��-   $��15��#��[ # � Cursor$
# Hand��$��0q�.���� $;��5/Agent.Interaction.Desktop;component/Images/Arrow.png��$
��Fill=�$��10��$��10��$��Bottom=�  $	 130 $	  120Q�$	! 150Q�" �� .R��� $��lblChatPersonName��$��Center=����� *ChatPersonName+  $ 25 $  25Q�$! 25Q�" �� .R��� $��imgChatStatusIcon��$��18��$��18������ *ChatPersonStatusIcon+  $	 100 $  90Q�$	! 120Q�" �� .R��� $��lblChatStatus��$��Center=����� *ChatPersonStatus+� $��1��
 �� $��* ��� ���� * SendchatConsultWindowRowHeight+.+�]� -   $��rtbViewChatWindow��$��0��$��Stretch=�$	��120��$	��150��
������ ��. .+�B   <Pointel.Interactions.Chat.CustomControls.BindableRichTextBox  -   $  Name$$ mainRTB��B	   <Pointel.Interactions.Chat.CustomControls.BindableRichTextBox"% 	  PreviewMouseRightButtonDown$)% #mainRTB_PreviewMouseRightButtonDown��& 	  TextChanged$& mainRTB_TextChanged��' �� 
IsReadOnly' . $��2q�( �� IsDocumentEnabled( . ) J� IsManipulationEnabled) .  * J� 	AllowDrop* . "+ �� VerticalScrollBarVisibility$+ Visible=�$
��Auto=�$��1q����#D6D7D6,   	Documents, �� *RTBConsultDocument+  - �� 
TargetType. �� Property/ �� Value0 �� Path1 �� RelativeSource2 �� Triggers3 P� Property4 P� Value  VerticalScrollBar��%h  '��       'J� O     '�� x     7  �� "- ��.��� . �@�ToolTip/ �� $0 NavigateUri�1 �� *Self+�� "- J�.��� . �@��Margin$/ 2q��� "- ��.��� . �@J�SnapsToDevicePixels/ . �� . �@�OverridesDefaultStyle/ . 2 P� 3 �@��Orientation$4 Vertical=�. ��� . � ��$/ 18���� . � ��$
/ Auto���� . � ��8/   5 � ContextMenu5 �� ]� -   $��dckSendChatWindow��$
��Auto��$��1��$��Bottom=�.+��� $��1q����#D6D7D6$��2q�._�� $��75��$	��Top=� �� $	��10* ��� $��48 ��� $��11 �.+��� -   $��txtChatSend��$��0,0,0,1q�
�������6 �� AcceptsReturn6 . $��0��$
��Auto��$��Stretch=�$7 �� HorizontalScrollBarVisibility$
7 Auto=�$
+ Auto=�  controltempTextbox#��[ $��2q�5 Z� {��� *IsTextMessageEnabled+  ��%�   '��       7  �� "- ��.��� . �@J�SnapsToDevicePixels/ . �� . �@�OverridesDefaultStyle/ . 2 P� 3 �@��Orientation$4 Vertical=�. ��� . � ��$/ 18���� . � ��$
/ Auto���� . � ��8/   �� -   $��stkSendPanel��$
��Auto��$��1��$��2��.+��� -   $��btnSendMessage��$��Stretch=�$��70��$��55��$��2,2,2,1q�$��Right=�  ChatSendButton#��[ $
��Send��$��Normal"�{��� *IsButtonConsultSendEnabled+