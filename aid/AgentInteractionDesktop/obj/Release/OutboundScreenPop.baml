   M S B A M L   `   `   `  ���� U  QAgent.Interaction.Desktop, Version=5.0.3.30, Culture=neutral, PublicKeyToken=nullU/clr-namespace:Agent.Interaction.Desktop.Helpers!Agent.Interaction.Desktop.Helpers  1    +Agent.Interaction.Desktop.OutboundScreenPop   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6my-http://schemas.microsoft.com/wpf/2008/toolkit  X TXceed.Wpf.Toolkit, Version=2.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f45xceed)http://schemas.xceed.com/wpf/xaml/toolkit  >DateTime/clr-namespace:Agent.Interaction.Desktop.Helpers  -     � Title$  OutboundScreenPop��$	��700��$	��400�� � WindowStyle$
 None=� � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico�� � AllowsTransparency . 
������  � WindowStartupLocation$ CenterOwner=� � SizeToContent$ Height=� � 	Resources  �� MergedDictionaries �� Source	 �� 
TargetType   	baseStyle
 �� Property �� Value �� BasedOn��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%J   &	        '�� #     7   �� "	 ��.��� 
 � ��$ 12(��� "	 ��8   .��� .+��� -   $��
MainBorder��$��0��$
��0.89q�$��8q�
�������._��  � RowDefinitions �� $��28 ��� $
��Auto ��� $
��Auto ��� $��* � � ColumnDefinitions �� �� .+��� -   ���#007edf$��2��._��� $��Outbound Call Details��
�������$��14(��� $��Contact Data��$��2q����#ADAAAD$��1q�$��Calibri)�$��3��$��0��$��1��.��)  #Microsoft.Windows.Controls.DataGrid  -   $��DGOutboundContactData��$��Stretch=�   RowHeaderWidth$ 0Q�$��5q�
�������$��Stretch=�$
��Left=�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserSortColumns .    
IsReadOnly .  %� IsVirtualizing .  %� VirtualizationMode$ 	Recycling=�$
��Auto=����� *NotifyCallDataView+   Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$ 85    
IsReadOnly . 4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *Key+  $ Value��$ *  .  �� *Value+   RowStyle �� ,  &Microsoft.Windows.Controls.DataGridRow"	  .��� 
 � ��$ 0q� �� Triggers P�   P� Property  �@ 
IsSelected! P� Value! . . ��� 
 � ��  HighlightedBrush# [ �� 
 � ��
 �   ��� $��Call Resolution��$��2q�$��80�����#ADAAAD$	��Top=�$��1q�$��Calibri)�$��1��$��1��.���� " �� Orientation$" Vertical=�.+��� $" 
Horizontal=�$��Stretch=�.+��� -   $��lblCallResult��$��Call Result:��$��2q�$��23���� -   $��cmbCallResult��$��2q�$��23��# �� DisplayMemberPath$# CallResultKeyName��$ �� SelectedValuePath$$ CallResultKeyValue������ *CallResultItemSource+�� $��Center=�$" 
Horizontal=�$��0,5,0,0q�.+��� -   $��chkbxRescheduleCall��$��Reschedule Call��$��14,5,0,5q��� -   $��chkbxPersonalCallback��$��Personal Callback��$��14,5,0,5q��� $��
Reschedule��$��2q����#ADAAAD$��1q�$��Calibri)�$��1��$��2��.���� $" 
Horizontal=�$��6,0,0,0q�.+��� $��Date:��(  "System.Windows.Controls.DatePicker  -	   $��dpRescheduledate���� $��Time:��$��30,0,0,0q�&   Xceed.Wpf.Toolkit.DateTimeUpDown  -
   $��dpRescheduletime��$	��100��%   Format$% LongTime=�,	  &Xceed.Wpf.Toolkit.Primitives.InputBase& 	  	Watermark$& Select a time��$
��Left=�$��5,2,2,2q��� -   $��	btnUpdate��$��23��$��10q�$��1��$��3��$��Right=�$��Bottom=�$��70��  NormalButton#��[ $��Update��{��� *IsEnableOutboundUpdate+