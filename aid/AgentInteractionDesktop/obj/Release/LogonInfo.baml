   M S B A M L   `   `   `  ���� U  QAgent.Interaction.Desktop, Version=5.0.3.30, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls  )    #Agent.Interaction.Desktop.LogonInfo   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     -     � Title$  	LogonInfo�� � AllowsTransparency .  � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � ShowInTaskbar .   � Topmost . $
��Auto��$	��320�����#656d90$��1q� � SizeToContent$ Height=� � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico��	 � 	Resources	 
 �� MergedDictionaries �� Source�� 
 �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%    .���  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -   $��lblTitle��$��0��$��14(�$��Stretch=�$��Calibri)����#007edf
�������$��Log On Information���� $��1��$��2�����#656d90$	��0.5q�$��5q��� -   $��gbVoiceChannel��$��10q�$��1��$��50�����#ADAAAD$	��0.5q����� -    ��Name$ 
chkbxVoice��   KeyVoice#���   $��15��{�.  d� 	IsChecked �� *IsVoiceChecked+.��� $��2q�$
��Auto�� �� ���� *LoginQueueRowHeight+�� ���� *AgentLoginIDRowHeight+�� ���� *AgentPasswordRowHeight+ � ColumnDefinitions �� $
��158* ��� $
��234* �.+��� -   $��lblQueue��  KeyQueue#���  $��0��$��0��{���  �� ElementName$ 
chkbxVoice�� �� Path$ CheckBox.IsChecked��� -   $��cbQueue��$��23��$
��Left=�$	��Top=�$��1�� �� 
IsEditable . $��1,0,0,0q����� *LoadCollection+ �� SelectedValue �� *QueueSelectedValue+ �� Mode$ TwoWay=�{��� $ 
chkbxVoice��$ CheckBox.IsChecked��� -   $��lblAgentLogin��$��Center=�  KeyAgentLogin#���  $��1��$��0��{��� $ 
chkbxVoice��$ CheckBox.IsChecked��� -   $��lblAgentLoginID��$�� ��$��1��$��1��$��2��{��� $ 
chkbxVoice��$ CheckBox.IsChecked��� -	   $��lblAgentPassword��$��Center=�  KeyAgentPassword#���  $��2��$��0��{��� $ 
chkbxVoice��$ CheckBox.IsChecked�G� -
   $��txtAgentPassword��$��2��$��1��$��Stretch=�$��2��  controltempPasswordbox#��[ $��2q�{��� $ 
chkbxVoice��$ CheckBox.IsChecked��� -   $��btnOk��$��2��$��0,10,105,10q�$��Right=�$��Apply��$��23��$��80��  NormalButton#��[ �� -   $��	btnCancel��$��2��$��
0,10,10,10q�$��Right=�$��Cancel��$��23��$��80��#��[ 