   M S B A M L   `   `   `  ���� S  OPointel.TaskbarNotifier, Version=5.0.2.50, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifierPointel.TaskbarNotifier  ! Pointel.Interactions.Outbound5   /Pointel.Interactions.Outbound.WinForms.Notifier   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     [NotifierFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifier     6my-http://schemas.microsoft.com/wpf/2008/toolkit  R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � AllowsTransparency  .  � WindowStyle$
 None=�
������  � Title$ Notifier��$
��Auto��$	��400�� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries �� Source��  �� $R L/Pointel.Interactions.Chat;component/Resources/StylesResourceDictionary.xaml?�%    .��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
����������#0070C5 J� BitmapEffect F�  F� Opacity$	 0.5Q�	 F� Softness$		 0.5Q�
 F� ShadowDepth$
 0Q� F� Color$ #003660��._��  � RowDefinitions �� $
��Auto ��� �� $
��Auto �.+���  �� Orientation$ 
Horizontal=�$��0�����#007edf$��2q�.+��� $@��:/Agent.Interaction.Desktop;component/Images/ChatNotify.png��$��15��$��15��$��5,2,5,2q��� $��14(�$��Calibri)�
����������� *	TitleText+9� -   $��1��$��2q�   SimpleExpanderTemp#��[   9� 
IsExpanded . $��RightToLeft=�$	��100������ $��	Case Data��$	��100��$��Calibri)����#0878ef$��Right=�$��Right=�.��� -   $��expGrid��$
��Auto�����#1d4380$��5,0,5,0q�.+�)  #Microsoft.Windows.Controls.DataGrid  -   $��DGChatCaseData��   ColumnWidth8  2Microsoft.Windows.Controls.DataGridLengthConverter$ *    RowHeaderWidth$ 0Q�$��LeftToRight=�
�������$	��Top=�$��Stretch=�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     
IsReadOnly .    CanUserDeleteRows .  $
��Auto=����� *NotifyCaseData+   Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��   Width$ 85    
IsReadOnly . 4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *Key+  $ Value��$ *  .  �� *Value+�� -   $��btnRight��$��0,5,5,5q�$��2��$��Right=�$��70��$��30���� -   $��btnLeft��$��0,5,80,5q�$��2��$��Right=�$��70��$��30��  
CallButton#��[ 