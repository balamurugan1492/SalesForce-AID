   M S B A M L   `   `   `  ���� S  OPointel.TaskbarNotifier, Version=5.0.1.50, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifierPointel.TaskbarNotifier   Pointel.Interactions.Chat1   +Pointel.Interactions.Chat.WinForms.Notifier   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     [NotifierFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifier     6my-http://schemas.microsoft.com/wpf/2008/toolkit  R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $��chatNotifier��  � AllowsTransparency  .  � WindowStyle$
 None=�
������  � Title$ Notifier��$
��Auto��$	��400�� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries �� Source��  �� $R L/Pointel.Interactions.Chat;component/Resources/StylesResourceDictionary.xaml?�%    .��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
����������#0070C5 J� BitmapEffect F�  F� Opacity$	 0.5Q�	 F� Softness$		 0.5Q�
 F� ShadowDepth$
 0Q� F� Color$ #003660��._��  � RowDefinitions �� $
��Auto ��� �� $
��Auto �.+�� $��0�����#007edf$��2q� � ColumnDefinitions �� $
��Auto ��� -    �Name$ grdTitle��$��* �.+��� $@��:/Agent.Interaction.Desktop;component/Images/ChatNotify.png��$��15��$��15��$��Center=�$��Center=�$��5,0,2,0q�$��0���� -   $��lblTitleStatus��$��14(�$��Calibri)�
�������$��Stretch=�$��CharacterEllipsis=�$��1��$	��330������ *	TitleText+9� -   $��CaseDataExpander��$��1��$��2q�   SimpleExpanderTemp#��[   9� 
IsExpanded . $��RightToLeft=�$	��125������ $��	Case Data��$	��355��$��Calibri)����#0878ef$��Right=�$��Right=�.��� -   $��expGrid��$
��Auto�����#1d4380$��5,0,5,0q�.+�)  #Microsoft.Windows.Controls.DataGrid  -   $��DGChatCaseData��   ColumnWidth8  2Microsoft.Windows.Controls.DataGridLengthConverter$ *    RowHeaderWidth$ 0Q�$��LeftToRight=�
�������$	��Top=�$��Stretch=�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     
IsReadOnly .    CanUserDeleteRows .  $
��Auto=����� *NotifyCaseData+   Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��   Width$ 85    
IsReadOnly . 4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *Key+  $ Value��$ *  .  �� *Value+�� -   $��ChatNotifier��$��1��$��2q�$	��125��._�� -	   $��grd_Details��$��1��
�������$��1,10,1,1q� J� Opacity$	 0.9Q� �� $��70 ��� $	��275��.+��� -
   $��MessageIconImageSource��$��Center=�$��Center=�$��50��$��50��� -   $��Details��$��1��$��5,0,0,0q�$��Center=�$��1�� �� $��80 ���  �� .+��� -   $��Message��$��Message��$��12(�$��Calibri)�$
��Bold"�$��0��$��0���� -   $��
Message111��$��12(�$��Calibri)�$
��Wrap=�$��0��$��1������ *NotifyMessage+�� -   $��btnRight��$��0,5,5,5q�$��2��$��Right=�$��70��$��30���� -   $��btnLeft��$��0,5,80,5q�$��2��$��Right=�$��70��$��30��  
CallButton#��[ 