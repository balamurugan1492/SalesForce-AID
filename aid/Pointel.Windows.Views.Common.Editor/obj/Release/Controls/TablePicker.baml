   M S B A M L   `   `   `  ���� '  #Pointel.Windows.Views.Common.Editor>    8Pointel.Windows.Views.Common.Editor.Controls.TablePicker   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -   
�������  � 	Resources     WindowBackgroundBrush �� Color  NormalBrush �� 
StartPoint �� EndPoint � Color � Offset  HighlightedBrush  NormalBorderBrush  HorizontalNormalBrush  HorizontalNormalBorderBrush  NormalButton �� 
TargetType �� Property �� Value	 �� 
TargetType
 �� CornerRadius J� SnapsToDevicePixels �� Triggers P� Property P� Value �� 
TargetName :� RoutedEvent �� TargetProperty W� From	 W� To g� Duration g� AutoReverse  controltempTextbox �� Padding ��Name  ComboBoxToggleButton � ColumnDefinitions ��Name F�Name 	 ComboBoxTextBox 
 ScrollBarLineButton �� Path �� RelativeSource  ScrollBarPageButton  ScrollBarThumb  VerticalScrollBar � RowDefinitions ]� DecreaseRepeatButton ]� Thumb  ]� IncreaseRepeatButton  HorizontalScrollBar! �� 	ClickMode" d� 	IsChecked# �� Mode$ J� IsHitTestVisible% �� SelectionBoxItem& �� SelectionBoxItemTemplate' ��Name( �� 	UndoLimit) �� 
IsReadOnly* �� 
IsReadOnly+ �� IsDropDownOpen, � AllowsTransparency- � ActualWidth. �� MaxDropDownHeight/ �� Triggers0 K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation1   DirectionalNavigation  DisabledBackgroundBrush2 P� 
SourceName��%  &	        &	      &	 f     &	 y     &	 �     &	 %    &	 {    '�� *    &	 �    &	 G    &		 �    &	
 �    &	 �    &	 �    &	 C
    7 
 7  7  7  7  7  7 
 &	 �    7 
 7  7  7  7  7  7 
 '�� �    7  '��     '�� )    7  7   7  7  7  �� $
 #FFF���� $	 0,0&�$	 0,1&���� � $
 #FFF��$	 0.0Q�� $
 #CCC��$	 1.0Q��� $ #CEE7F7���� $	 0,0&�$	 0,1&���� � $
 #CCC��$	 0.0Q�� $
 #444��$	 1.0Q��� $	 0,0&�$	 1,0&���� � $
 #FFF��$	 0.0Q�� $
 #CCC��$	 1.0Q��� $	 0,0&�$	 1,0&���� � $
 #CCC��$	 0.0Q�� $
 #444��$	 1.0Q��� " ��.���  � �� �� "	 ��.R��� $��Border��$
 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. # z #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#007edf:� $ Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$ 1X�$
 0.25X�$ 0:0:0.25D� . P�  � �� . . ��� $ Border�� � �� �#0061ac�� " ��.���  � ��
 �   ���  � ��$ Calibri)���  � ��$ 12(���  � �� �� "	 ��.R��� #��z��._��� $��5q�#��z��#��z��# z ��.  �� "	 ��.R��� $ 2q�
����������#ADAAAD$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� "	 d�.R��  �� �� $��20 �.+��� $ Border��$��2��
����������#ADAAAD$��1q��� $��0��$��1q�
������ $��0,0,0,0q�F� $ Arrow��$��1�����#737573$��Center=�$��Center=�"����        � 	=  	= � z     	 P�  �@J�IsMouseOver . P�  �@d�	IsChecked$
 trueY�P�  � {� .  �� "	 ��.R��� $ PART_ContentHost��|�.  #��z��$��0q�
������ �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � |� .  ��  � �� �� "	 ��.R��� $��Border��$��1q�
����������#ADAAAD$��1q�._�F� $��Center=�$��Center=����#737573���� $ Content� �� *TemplatedParent+ P�  �@J�IsMouseOver . . ��� $ Border�� � ��
 �   �P�  � �� . . ��� $ Border�� � ��
 �   ��� $ Border�� � �� �#F7F3F7P�  � {� .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� "	 ��.R��� ���#F7F3F7�� " n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� "	 n�.R��� $��Border��
����������#ADAAAD$��1q� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#F7F3F7P�  �@n�
IsDragging . . ��� $ Border�� � �� �#F7F3F7�� $ Border�� � ��
 �   �P�  � {� .  �� "	 ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����.  �� 8�� $��ScrollBar.PageUpCommand�� n� 8�� $��1,0,1,0q�8�� 8��   �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� "	 ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.   �� 8�� $��ScrollBar.PageLeftCommand�� n� 8�� $��0,1,0,1q�8�� 8��   �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� " ��.���  � �� �#737573��  � ��$ 20����  � ��$ Center=���  � �� �� "	 ��.R��� $ Bd�� . #��z��#��z��#��z��# z��._��� #��z��#��z��# z  P�  �@��IsHighlighted . . ��� 8    � ����  � ��$ 12(�P�  � {� . . ���  � ��$ 12(�P�  � {� .  �� " ��.���  � ��
 �   ��� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  � ��
 �   ���  � �� �� "	 ��.R�� .+�d� $��ToggleButton��8��  |�.  $! Press=�" �� $ IsDropDownOpen�$# TwoWay=� �� *TemplatedParent+�� $��ContentSite��$ .  #��z% #��z& #��z��$��3,3,23,3q�$��Center=�$
��Left=��� $' PART_EditableTextBox��$( 10��$��LeftToRight=�$	��210��$
��Left=�
������ $��0q�$	��Top=�$��25��$
��Left=�$	��Top=�$��3,3,23,3q�|�. 
������ $y�Hidden=�#* z) ��Z� � $��Popup��$��Bottom=�#��z+ , . |�.  $��Slide=�.��� $��DropDown�� . #��z- #��z. $
��Auto��.+��� $ DropDownBorder��8�� $��1,0,1,1q����#ADAAAD�� $��4,6,4,6q� .   ���� '��       " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . / P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  .���� 0 . $1 	Contained=� P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$ 95��P�  � {� .  . ��� $ PART_EditableTextBox��8   � ���� $ ToggleButton�� � {� .  P�  � {� . . ��� $ ToggleButton�� � {� . P�  �@��
IsGrouping . . ���  � �� .  P� $2 Popup�� �@�AllowsTransparency . . ��� $ DropDownBorder�� � ��$ 0,0,0,0q�P�  �@��
IsEditable . . ���  � �� .  �� $ PART_EditableTextBox�� � y�$ Visible=��� $ ContentSite�� � y�$ Hidden=�/ .��� $
��Auto��.+�]� .+��� $	��Top=�._��� 3 �� Orientation$3 
Horizontal=�$
��Left=�$��4q�.+��� $E��?/Pointel.Windows.Views.Common.Editor;component/Images/table.png��$��2,0,0,0q��� $��Insert Table��$��2,0,0,0q�$��14(�$
��Bold"��� $��Bottom=�$ 12q�._��� $3 
Horizontal=�$��Right=�.+��� -   4 ��Name$4 
OkayButton��$��OK��$��56��$��25��$	��8,0q�#��[ 5   TabIndex$5 14��6   TabNavigation$6 Local=��� -   $4 CancelButton��$��Cancel��$��56��#��[ $5 15��$6 Local=��� $ 8q�._�� $��2q� �� $
��Auto ��� $	��180 ��� $��10 ��� $
��Auto ��� $	��120 � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $
��Rows��$��0��$��0��$��2q��� $��Columns��$��3��$��0��$��2q��� $��Width��$��0��$��1��$��2q��� $��Height��$��3��$��1��$��2q��� $��Spacing��$��0��$��2��$��2q��� $��Padding��$��3��$��2��$��2q��� $��Border Color��$��3��$��2,2,2,0q�$��31��$	��Top=��� $��Title��$��3��$��3��$��2q��� $��Background Color��$��0��$��4��$��2q��� $��	Alignment��$��3��$��4��$��2q��� -   $��txtRows��$( 10��$��1��$��0��$��2q�#��[ $5 0��$6 Local=��� -   $��
txtColumns��$( 10��$��4��$��0��$��2q�#��[ $5 1��$6 Local=�]� $��1��$��1��.+��� -   7 ��Name$7 WidthUnitSelection��$��40��$��2q�8 �� SelectedIndex$8 0��$5 2��$6 Local=�.@��� .��px�� -   $��txtWidth��$( 10��#��[ $��2q�$5 3��$6 Local=�]� $��4��$��1��.+��� -   $7 HeightUnitSelection��$��40��$��2q�$8 0��$5 4��$6 Local=�.@��� .��px�� -	   $��	txtHeight��$( 10��#��[ $��2q�$5 5��$6 Local=�]� $��1��$��2��.+��� -
   $7 SpaceUnitSelection��$��40��$��2q�$8 0��$5 6��$6 Local=�.@��� .��px�� -   $��
txtSpacing��$( 10��#��[ $��2q�$5 7��$6 Local=�]� $��4��$��2��.+��� -   $7 PaddingUnitSelection��$��40��$��2q�$8 0��$5 8��$6 Local=�.@��� .��px�� -   $��
txtPadding��$( 10��#��[ $��2q�$5 9��$6 Local=��� $��1��$��3��$��2q����#c6c7c6$��1q�._��� $3 
Horizontal=�$��2q�.+��� -   $��showBorderColor��$��30��$��15��$��1q��� -   $7 cbBorder��$��1q�$	��150��$5 10��$6 Local=��� -   $��txtTitle��$( 10��$��4��$��3��#��[ $��2q�$5 11��$6 Local=��� $��1��$��4��$��2q����#c6c7c6$��1q�._��� $3 
Horizontal=�$��2q�.+��� -   $��showBGColor��$��30��$��15��$��1q��� -   $7 	cbBGColor��$��1q�$	��150��$5 12��$6 Local=��� -   $7 AlignmentSelection��$��4��$��4��$��2,0,2,2q�$��31��$5 13��$6 Local=�.@��� .��Center�� .��	Justify�� .��Left�� .��Right