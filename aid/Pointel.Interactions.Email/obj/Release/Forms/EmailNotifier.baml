   M S B A M L   `   `   `  ���� S  OPointel.TaskbarNotifier, Version=5.0.2.50, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifierPointel.TaskbarNotifier   Pointel.Interactions.Email4   .Pointel.Interactions.Email.Forms.EmailNotifier   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     [NotifierFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifier     6my-http://schemas.microsoft.com/wpf/2008/toolkit  -     � AllowsTransparency  .  � WindowStyle$
 None=�
������  � Title$ Notifier��$
��Auto��$	��400�� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries �� Source��  �� $S M/Pointel.Interactions.Email;component/Resources/StylesResourceDictionary.xaml?�%    .��� .+��� -   $��
MainBorder��$
��0.89q�$��8q�
����������#0070C5 J� BitmapEffect F�  F� Opacity$	 0.5Q�	 F� Softness$		 0.5Q�
 F� ShadowDepth$
 0Q� F� Color$ #003660��._��  � RowDefinitions �� $
��Auto ��� �� $
��Auto �.+�� $��0�����#007edf$��2q� � ColumnDefinitions �� $
��Auto ��� -    �Name$ grdTitle��$��* �.+��� $F��@/Agent.Interaction.Desktop;component/Images/Email/emailwhite.png��$��0��$��15��$��15��$��5,0,2,0q�$��Center=� � DataContext �� �� $��14(�$��Calibri)�
�������$��5,0,5,0q�$��1��$��CharacterEllipsis=�$	��325������ *	TitleText+9� -   $��1��$��2q�   SimpleExpanderTemp#��[   9� 
IsExpanded . $��RightToLeft=�$	��125������ $��	Case Data��$	��100��$��Calibri)����#0878ef$��Right=�$��Right=�.��� -   $��expGrid��$
��Auto�����#1d4380$��5,0,5,0q�.+�)  #Microsoft.Windows.Controls.DataGrid  -   $��dgEmailCaseData��   ColumnWidth8  2Microsoft.Windows.Controls.DataGridLengthConverter$ *    RowHeaderWidth$ 0Q�$��LeftToRight=�
�������$	��Top=�$��Stretch=�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     
IsReadOnly .    CanUserDeleteRows .  $
��Auto=����� *EmailCaseData+   Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��   Width$ 85    
IsReadOnly . 4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *Key+  $ Value��$ *  .  �� *Value+�� $��2��$��Right=� �� Orientation$ 
Horizontal=�.+��� -   $��	btnAccept��$��0,5,5,5q�$��Right=�$��_Accept��$��70��$��30��  
CallButton#��[ �� -   $��	btnReject��$��0,5,5,5q�$��Right=�$��_Reject��$��70��$��30��  RejectButton#��[ 