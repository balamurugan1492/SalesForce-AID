   M S B A M L   `   `   `  ���� p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls  U QAgent.Interaction.Desktop, Version=5.0.3.30, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls +   %Agent.Interaction.Desktop.RefinePlace   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     cmyTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit     PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     -   $��RefineWindow��  � Title$  Login�� � WindowStyle$
 None=� � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico�� � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�$	��400��$	��400�� � ShowInTaskbar .   � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries	 �� Source��  �� $R	 L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%    .��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
�������._�� 
 � RowDefinitions
 �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+�� $��0��
�������.+��� $��5,0,0,0q�
�������$��Center=� �� Orientation$ 
Horizontal=�.+��� -   $��image1��$
��Auto��$��40��$G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png���� $ Vertical=�.+��� $��18(�$��0,3,0,0q����#007edf$
��Bold"�   keyTitle#���   $��Calibri)��� -   $��channelTitleversion��$��
140,-7,0,3q�  keyTitleVersion#���  $��Calibri)�$��12(��� $��0.5,0.5,0.5,0q����#656d90$��5,0,5,0q�$��1��._��  � ColumnDefinitions �� $
��161* ��� $
��237* �
 �� $	��43* ��� -    �Name$ grdrow_cserror������ *CSErrorRowHeight+�� $��1* ��� $	��23* ��� -    ��Name$ grdRowAgentlogin��$	��23* ��� -   $ grdRowEmail��$
��Auto ��� -   $ 
grdRowChat��$
��Auto ��� -	   $ grdRowOutbound��$
��Auto �.+��� -
   $��lblUserName��  
KeyWelcome#���  $��2��$��2q����#E1F2FA�� -   $��label1��  KeyWelcomeinfo#���  $��2,20,0,0q�$��2���� -   $��lblPlace��  KeyPlace#���  $��2��$��	10,10,0,0q��� -   $��
stkp_Error��$ 
Horizontal=�$��1��$��2��$��3,0,3,3q�
��� ���$��Stretch=�.+��� $��5,2,0,2q�$	��Top=�$;��5/Agent.Interaction.Desktop;component/Images/Error.png��$��20��$��20���� -   $��lblInformation��$��10,5,5,5q�$	��300��$
��Wrap=����� *CSErrorMessage+@  :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx  -     Name$ txtPlace��@  :Agent.Interaction.Desktop.CustomControls.AutoCompleteBoxEx   SelectionChanged$ txtPlace_SelectionChanged��   PreviewKeyUp$ txtPlace_PreviewKeyUp��   GotFocus$ txtPlace_GotFocus��$��2��$��2��$��2,2,12,2q�$��23�����#ADAAAD
�������
���   �$��Calibri)�$��12(�$��Center=�$��Stretch=�-   'System.Windows.Controls.AutoCompleteBox   SelectedItem �� *Place+ �� Mode$ TwoWay=�   Text �� *Place+   ItemsSource �� *AutoComplete+   ItemTemplate �� .R��� $��innertxt��
���   �$	��190��$��2q�$��Calibri)�$��12(�����   �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value  HorizontalScrollBar  VerticalScrollBar��%z  '�       '�� #     7  7  �� " �.���  � ��$ 85���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8   P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  �� -   $��gbVoiceChannel��$��5q�$��3��$��2�����#ADAAAD$��1q����� -    ��Name$ 
chkbxVoice��  KeyVoice#���  $��15��{�.  d� 	IsChecked �� *IsVoiceChecked+.��� -   $��grd_VoiceLoginDetails��$��1q�$
��Auto��
 �� ���� *LoginQueueRowHeight+�� ���� *AgentLoginIDRowHeight+�� ���� *AgentPasswordRowHeight+�� ���� *WrapTimeRowHeight+ �� $
��140* ��� $
��166* ��� $	��41* �.+��� -   $��lblQueue��  KeyQueue#���  $��0��$��0��{���   �� ElementName$  
chkbxVoice��! �� Path$! CheckBox.IsChecked��� -   $��cbQueue��$��23��$
��Left=�$	��Top=�" �� SelectedIndex$" 0��$��1��# �� 
IsEditable# . $��2������ *LoadCollection+$ �� SelectedValue$ �� *QueueSelectedValue+$ TwoWay=�{��� $  
chkbxVoice��$! CheckBox.IsChecked��� -   $��lblAgentLogin��$��Center=� 	 KeyAgentLogin#��� 	 $��1��$��0��{��� $  
chkbxVoice��$! CheckBox.IsChecked��� -   $��lblAgentLoginID��$�� ��$��1��$��1��$��2��{��� $  
chkbxVoice��$! CheckBox.IsChecked��� -   $��cbAgentLoginID��$��23��$��1��$��1��$y�Hidden=�$" 0��$��2��$	��0,1q����� *AgentLoginIds+�� -   $��lblAgentPassword��$��Center=� 
 KeyAgentPassword#��� 
 $��2��$��0��{��� $  
chkbxVoice��$! CheckBox.IsChecked�G� -   $��txtAgentPassword��$��2��$��1��$��23��$��Stretch=�$��2��  controltempPasswordbox#��[ {��� $  
chkbxVoice��$! CheckBox.IsChecked��� -   $��lblWrapupTime��$��Center=�  KeyWrapTime#���  $��3��$��0��{��� $  
chkbxVoice��$! CheckBox.IsChecked��� -   $��txtWrapupTime��$��3��$��1��$��23��$��Stretch=�$��2��  controltempTextbox#��[ {��� $  
chkbxVoice��$! CheckBox.IsChecked����� *
WrapTime+�� -   $��
chkbxEmail��$��5��$��email��$��14,5,0,5q�$y�Hidden=� �� *IsEmailChecked+�� -   $��	chkbxChat��$��6��$
��chat��$��14,5,0,5q�$y�Hidden=� �� *IsChatChecked+�� -   $��chkbxOutbound��$��7��$��outboundpreview��$��8,0,0,0q�$��14,5,0,5q�$y�Hidden=� �� *IsOutboundChecked+�� ���#656d90$��0.5,0.5,0.5,0.5q�$��5,0,5,0q�$��2��._�� $��30��.+��� -   $��	btnCancel��$��23��$��70��$��Right=�$��0,0,10,0q�$��Center=�  NormalButton#��[ �� -    $��btnOk��$��23��$��Right=�$��0,0,90,0q�$��Center=�$��70��#��[ �� $��30�����#007edf$��Stretch=�$	��Top=�$��0,3,0,0q�$��4��._�� .+��� $��© 2013-2017 Pointel Inc��$
��Left=�$��Center=�$��Calibri)�$��12(�
�������$��5,0,0,0q��� $��Right=�$��Center=����#EA3737% �� CornerRadius$% 	10,0,0,10��y��� *TrialVisibility+._��� $��Calibri)�$��Center=�$��Center=�$��12(�
�������$��	15,2,10,2q����� *TrailMessage+