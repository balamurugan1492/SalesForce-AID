   M S B A M L   `   `   `  ���� '  #Pointel.Windows.Views.Common.Editor>    8Pointel.Windows.Views.Common.Editor.Controls.ImagePicker   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     
�������  � 	Resources     NormalImageButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius J� SnapsToDevicePixels �� Triggers P� Property	 P� Value
 �� 
TargetName  WindowBackgroundBrush �� Color  NormalBrush �� 
StartPoint �� EndPoint � Color � Offset  HighlightedBrush  NormalBorderBrush  HorizontalNormalBrush  HorizontalNormalBorderBrush  NormalButton :� RoutedEvent �� TargetProperty W� From	 W� To g� Duration g� AutoReverse  controltempTextbox �� Padding ��Name 	 ComboBoxToggleButton � ColumnDefinitions ��Name F�Name 
 ComboBoxTextBox  ScrollBarLineButton �� Path �� RelativeSource  ScrollBarPageButton  ScrollBarThumb  VerticalScrollBar � RowDefinitions ]� DecreaseRepeatButton ]� Thumb  ]� IncreaseRepeatButton  HorizontalScrollBar! �� 	ClickMode" d� 	IsChecked# �� Mode$ J� IsHitTestVisible% �� SelectionBoxItem& �� SelectionBoxItemTemplate' ��Name( �� 	UndoLimit) �� 
IsReadOnly* �� 
IsReadOnly+ �� IsDropDownOpen, � AllowsTransparency- � ActualWidth. �� MaxDropDownHeight/ �� Triggers0 K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation1   DirectionalNavigation  DisabledBackgroundBrush2 P� 
SourceName��%  &	        &	 0    &	 @    &	 �    &	 �    &	 �    &	 U    &	 �    '�� Z    &	     &		 w    &	
 �    &	     &	 	    &	 �	    &	 s    7  7  7  7  7  7  7  &	 %    7  7  7  7  7  7  7  '�� �    7  '�� 4    7 	 7  7  7  7  �� " ��.���  � �� �� " ��.R��� $��Border��
������ $ 2�����#c6c7c6$��1q�._��� $��content��#��z��#��z����. # z #��z�� P�  �@J�IsMouseOver	 . . ��� $
 Border�� � �� �#d6ebffP�  � ��	 . . ��� $
 Border�� � �� �#94c3e7�� $
 #FFF���� $	 0,0&�$	 0,1&���� � $
 #FFF��$	 0.0Q�� $
 #CCC��$	 1.0Q��� $ #CEE7F7���� $	 0,0&�$	 0,1&���� � $
 #CCC��$	 0.0Q�� $
 #444��$	 1.0Q��� $	 0,0&�$	 1,0&���� � $
 #FFF��$	 0.0Q�� $
 #CCC��$	 1.0Q��� $	 0,0&�$	 1,0&���� � $
 #CCC��$	 0.0Q�� $
 #444��$	 1.0Q��� " ��.���  � �� �� " ��.R��� $��Border��$ 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. # z #��z�� P�  �@J�IsMouseOver	 . . ��� $
 Border�� � �� �#007edf:� $ Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$ 1X�$
 0.25X�$ 0:0:0.25D� . P�  � ��	 . . ��� $
 Border�� � �� �#0061ac�� " ��.���  � ��
 �   ���  � ��$ Calibri)���  � ��$ 12(���  � �� �� " ��.R��� #��z��._��� $��5q�#��z��#��z��# z ��.  �� " ��.R��� $ 2q�
����������#ADAAAD$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " d�.R��  �� �� $��20 �.+��� $ Border��$��2��
����������#ADAAAD$��1q��� $��0��$��1q�
������ $��0,0,0,0q�F� $ Arrow��$��1�����#737573$��Center=�$��Center=�"����        � 	=  	= � z     	 P�  �@J�IsMouseOver	 . P�  �@d�	IsChecked$
	 trueY�P�  � {�	 .  �� " ��.R��� $ PART_ContentHost��|�.  #��z��$��0q�
������ �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � |� .  ��  � �� �� " ��.R��� $��Border��$��1q�
����������#ADAAAD$��1q�._�F� $��Center=�$��Center=����#737573���� $ Content� �� *TemplatedParent+ P�  �@J�IsMouseOver	 . . ��� $
 Border�� � ��
 �   �P�  � ��	 . . ��� $
 Border�� � ��
 �   ��� $
 Border�� � �� �#F7F3F7P�  � {�	 .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " ��.R��� ���#F7F3F7�� " n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " n�.R��� $��Border��
����������#ADAAAD$��1q� P�  �@J�IsMouseOver	 . . ��� $
 Border�� � �� �#F7F3F7P�  �@n�
IsDragging	 . . ��� $
 Border�� � �� �#F7F3F7�� $
 Border�� � ��
 �   �P�  � {�	 .  �� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����.  �� 8�� $��ScrollBar.PageUpCommand�� n� 8�� $��1,0,1,0q�8�� 8��   �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.   �� 8�� $��ScrollBar.PageLeftCommand�� n� 8�� $��0,1,0,1q�8�� 8��   �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� " ��.���  � �� �#737573��  � ��$ 20����  � ��$ Center=���  � �� �� " ��.R��� $ Bd�� . #��z��#��z��#��z��# z��._��� #��z��#��z��# z  P�  �@��IsHighlighted	 . . ��� 8    � ����  � ��$ 12(�P�  � {�	 . . ���  � ��$ 12(�P�  � {�	 .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  � �� �� " ��.R�� .+�d� $��ToggleButton��8��  |�.  $! Press=�" �� $ IsDropDownOpen�$# TwoWay=� �� *TemplatedParent+�� $��ContentSite��$ .  #��z% #��z& #��z��$��3,3,23,3q�$��Center=�$
��Left=��� $' PART_EditableTextBox��$( 10��$��LeftToRight=�$	��210��$
��Left=�
������ $��0q�$	��Top=�$��25��$
��Left=�$	��Top=�$��3,3,23,3q�|�. 
������ $y�Hidden=�#* z) ��Z� � $��Popup��$��Bottom=�#��z+ , . |�.  $��Slide=�.��� $��DropDown�� . #��z- #��z. $
��Auto��.+��� $ DropDownBorder��8�� $��1,0,1,1q����#ADAAAD�� $��4,6,4,6q� .   ���� '��       " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . / P�  �@��Orientation$	 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  P�  �@��Orientation$	 Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  .���� 0 . $1 	Contained=� P�  �@��HasItems	 .  . ��� $
 DropDownBorder�� � ��$ 95��P�  � {�	 .  . ��� $
 PART_EditableTextBox��8   � ���� $
 ToggleButton�� � {� .  P�  � {�	 . . ��� $
 ToggleButton�� � {� . P�  �@��
IsGrouping	 . . ���  � �� .  P� $2 Popup�� �@�AllowsTransparency	 . . ��� $
 DropDownBorder�� � ��$ 0,0,0,0q�P�  �@��
IsEditable	 . . ���  � �� .  �� $
 PART_EditableTextBox�� � y�$ Visible=��� $
 ContentSite�� � y�$ Hidden=�/ .��� $
��Auto��.+��� ���#C6C7C6$��1q�$��2q�._�]� .+��� $	��Top=�._��� 3 �� Orientation$3 
Horizontal=�$
��Left=�$��4q�.+��� $E��?/Pointel.Windows.Views.Common.Editor;component/Images/image.png��$��2,0,0,0q��� $��Insert Image��$��2,0,0,0q�$��14(�$
��Bold"��� -   $ TopContentArea��$ 8q�$	��Top=�._�� .+�]� $��2q�.+��� $3 
Horizontal=�$��Right=�$��2q�.+��� -   4 ��Name$4 BrowseButton��$	��3,2q����#c6c7c6$��1q�5 � ToolTip$5 Browse��#��[  6   TabIndex$6 1��7   TabNavigation$7 Local=�.���� $3 
Horizontal=�.+��� $G��A/Pointel.Windows.Views.Common.Editor;Component/Images/browser.png��$��16��$��16���� $��Browse��$	��4,0q��� -   $' UrlText��$( 10��#��[ ���	#FFC5C5C5$��0q�$��3,2,3,1q�$��2q�$6 0��$7 Local=��� ���	#FFC5C5C5$��0,1,0,0q�$ 8q�$��Bottom=�._��� $3 
Horizontal=�$��Right=�.+��� -   $4 
OkayButton��$��56��$��25��$	��8,0q�$��OK��#��[ $6 11��$7 Local=��� -   $4 CancelButton��$��56��$��Cancel��#��[ $6 12��$7 Local=��� ���	#FFC5C5C5$��1q�$��Right=�$��3q�._��� $��2q�.+��  �� $
��Auto ��� $	��100 � �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��Width��$��0��$��2q��� $��Height��$��1��$��2q��� $��H Space��$��2��$��2q��� $��V Space��$��3��$��2q��� $��Border��$��4��$��2q��� $��	Alignment��$��5��$��2q��� $��Title��$��6��$��2q��� $��Alt Text��$��7��$��2q��� $��Link To��$��8��$��2q��� -   $��txtWidth��$( 10��$��0��$��1��#��[ $��2q�$6 2��$7 Local=��� -   $��	txtHeight��$( 10��$��1��$��1��#��[ $��2q�$6 3��$7 Local=��� -   $��	txtHSpace��$( 10��$��2��$��1��#��[ $��2q�$6 4��$7 Local=��� -	   $��	txtVSpace��$( 10��$��3��$��1��#��[ $��2q�$6 5��$7 Local=��� -
   $��	txtBorder��$( 10��$��4��$��1��#��[ $��2q�$6 6��$7 Local=��� -   8 ��Name$8 ImageAlignmentSelection��$��5��$��1��$��2q�$6 7��$7 Local=��� -   $��txtTitle��$( 10��$��6��$��1��$��3��#��[ $��2q�$6 8��$7 Local=��� -   $��
txtAltText��$( 10��$��7��$��1��$��3��#��[ $��2q�$6 9��$7 Local=��� -   $��	txtLinkTo��$( 10��$��8��$��1��$��3��$��2q�#��[ $6 10��$7 Local=��� $��10,2,2,2q����#C6C7C6$��1q�._�� $	��370��$	��400��$��2q�.+��� -   9 ��Name$9 StatusPrompt��$��Center=�$��
No Preview��$��Center=��� -   $ PreviewScroll��: K� ZIndex$: 1��$
��Auto=�$
��Auto=�  ��%M  '��       7  7  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . / P�  �@��Orientation$	 Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8   P�  �@��Orientation$	 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  .���� �� -   ; �Name$; imgURL��< �� TextDecorations$
< None{�.�.��� -   = ��Name$= PreviewImage��$
��Fill=����� $ Width�$# OneWay=����� $ Height�$# OneWay=��� -   > ��Name$> SliderGroup��$: 5��$��25q�$��Right=�$��Bottom=�.+��� -   $4 ZoomInButton��$5 ZoomIn��#��[  .���� $F��@/Pointel.Windows.Views.Common.Editor;Component/Images/ZoomIn.png��$��16��$��16���� -   ? ��Name$? ResizeSlider��@ �� Orientation$@ Vertical=�$��Center=�A �� IsSnapToTickEnabledA .  B � Cursor$
B Hand��$	��100��$��2q�C � Maximum$	C 100Q�D � Minimum$
D -100Q�E � LargeChange$E 10Q�F �� TickFrequency$F 25Q�G �� AutoToolTipPlacement$G BottomRight=�H �� TickPlacement$H BottomRight=��� -   $4 ZoomOutButton��$5 ZoomOut��#��[  .���� $G��A/Pointel.Windows.Views.Common.Editor;Component/Images/ZoomOut.png��$��16��$��16��