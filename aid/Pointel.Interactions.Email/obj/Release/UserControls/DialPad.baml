   M S B A M L   `   `   `  ���� U  QPointel.Interactions.Email, Version=5.0.3.6, Culture=neutral, PublicKeyToken=null]3clr-namespace:Pointel.Interactions.Email.Converters%Pointel.Interactions.Email.Converters  5    /Pointel.Interactions.Email.UserControls.DialPad   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6my-http://schemas.microsoft.com/wpf/2008/toolkit  Aconvert3clr-namespace:Pointel.Interactions.Email.Converters  -     � 	Resources   �� MergedDictionaries �� Source   GroupHeaderStyle �� 
TargetType �� Property �� Value �� 
TargetType 9�Name 9� 
IsExpandedD   >Pointel.Interactions.Email.Converters.DGRowForegroundConverter  RowForegroundB   <Pointel.Interactions.Email.Converters.DGRowIsEnableConverter  	RowEnable��  �� $S M/Pointel.Interactions.Email;component/Resources/StylesResourceDictionary.xaml?�%�   &	        &	 �     &	 �     �� " ��.���  � �� �� " ��.R�9� $	 exp�� . ���#007edf
����������� $��Calibri)�$
��Bold"�$��Normal&�
����������� *Name+.����     .��� $
��Auto��.+��� -   $��SelectionTab��	 K� ZIndex$	 0��$��	0,-3,0,-3q�$	��209��$	��300��.@��� -   $��tabDial��$��Calibri)����� 
 �� Orientation$
 
Horizontal=�.+��� $��12��$A��;/Agent.Interaction.Desktop;component/Images/Voice.Short.png���� -   $�� Dial��$��12(�$��Center=�$��Center=�.��� 
������� � ColumnDefinitions �� �� ��  � RowDefinitions �� $��48 ��� �� �� �� �� $��40 �.+��� -   $��
txtNumbers�� �� 	UndoLimit$ 10��$��0��$��3��$��5q� �� 
IsReadOnly . $��0q�
������ 
������ ���#007EDF$��Calibri)�$
��Bold"�$��35(�$��45��$��Center=����� *DialedNumbers+�� -   $
��btn1��$��1��$��0��$��1��$��1q�  NormalButton#��[ �� -   $
��btn2��$��1��$��1��$��2��$��1q�#��[ �� -   $
��btn3��$��1��$��2��$��3��$��1q�#��[ �� -	   $
��btn4��$��2��$��0��$��4��$��1q�#��[ �� -
   $
��btn5��$��2��$��1��$��5��$��1q�#��[ �� -   $
��btn6��$��2��$��2��$��6��$��1q�#��[ �� -   $
��btn7��$��3��$��0��$��7��$��1q�#��[ �� -   $
��btn8��$��3��$��1��$��8��$��1q�#��[ �� -   $
��btn9��$��3��$��2��$��9��$��1q�#��[ �� -   $
��btns��$��4��$��0��$��*��$��1q�#��[ �� -   $
��btn0��$��4��$��1��$��0��$��1q�#��[ �� -   $
��btnh��$��4��$��2��$��#��$��2,1,0,1q�#��[ � $��5��$��3�� �� �� .+��� -   $
��Call��$��1q�$��0�����#4DBF4D  
CallButton#��[ $
��Dial���� -   $��btnClear��$��1q�$��1��  ClearButton#��[ $��	Backspace���� -   $��tabPhoneBook��$��Calibri)����� $
 
Horizontal=�.+��� $��12��$?��9/Agent.Interaction.Desktop;component/Images/PhoneBook.png���� -   $�� Phone Book��$��12(�$��Center=�$��Center=�.��� 
������� �� $
��Auto ��� $
��211* �.+��� $��1��$��Contacts From��$��5q����#E3E3E3$��Calibri)�.���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� �� $
��Auto � �� �� .+��� -   $��ChkAgentLevel��$��0��$��5q�$��0��$��Local Contact��$��11(�
���  ���� -   $��ChkApplicationLevel��$��0��$��5q�$��1��$��Global Contact��$��11(�
����  ��� -   $��ChkGroupLevel��$��1��$��5q�$��0��$��Group Contact��$��11(�
��� � ��� $��0,2,0,2q�$��2��$��2��$��20�����#adaaad$	��0.5q�._��  �� �� $
��Auto �.+��� -   $��txtContactSearch��$ 10��
������ 
�������$��0,1,0,-1q��� $��1��$��2q�$<��6/Agent.Interaction.Desktop;component/Images/Search.png��$��15��)  #Microsoft.Windows.Controls.DataGrid  -   $��
dgvContact�����#adaaad$��3��$��Stretch=�$��Stretch=�$��2��   AutoGenerateColumns .     HeadersVisibility$ Column=�   CanUserAddRows .     CanUserDeleteRows .     RowHeaderWidth$ 0Q�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   
IsReadOnly .    Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$
 Name��   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$ 75    CanUserSort .    SortMemberPath$
 Name��4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *Name+   	CellStyle �� -  'Microsoft.Windows.Controls.DataGridCell"  .�-   ��  � ��$ 0q� �� Triggers P�  P� Property �@ 
IsSelected  P� Value  . . ���  � ��  HighlightedBrush# [ ��  � ��
 �   �  $ Number��$ *  .   �� *Number+ �� "  .�-   ��  � ��$ 0q� P�  �@ 
IsSelected  . . ���  � ��# [ ��  � ��
 �   �   .  !   
Visibility$! Hidden=� �� *Type+"   RowStyle" �� ,	  &Microsoft.Windows.Controls.DataGridRow" 	 .���  � �� `� # `� 	Converter## [ .1��� $ �� Path$
$ Type��� $$ Number���  � {� �� $$ Number�% �� 	Converter#% [  �� & �� Binding& �� $$ TextBlock.Foreground�' �� Value' 	LightGray.a���  � {� .  P�  �@	 
IsSelected  . . ���  � ��# [ ��  � ��
 �   ��� -   $��	lblStatus��$��4��$��2��$��11(�$��Stretch=�$��Center=�$��No contacts available��
����i��