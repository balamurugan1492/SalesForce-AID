   M S B A M L   `   `   `  ���� ^  ZPointel.Windows.Views.Common.Editor, Version=1.0.0.3, Culture=neutral, PublicKeyToken=nullk:clr-namespace:Pointel.Windows.Views.Common.Editor.Controls,Pointel.Windows.Views.Common.Editor.Controls  w@clr-namespace:Pointel.Windows.Views.Common.Editor.CustomControls2Pointel.Windows.Views.Common.Editor.CustomControls  =    7Pointel.Windows.Views.Common.Editor.Controls.HTMLEditor   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Llocal:clr-namespace:Pointel.Windows.Views.Common.Editor.Controls     ZCustomControl@clr-namespace:Pointel.Windows.Views.Common.Editor.CustomControls     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -   $��
htmleditor��.��� .+��� -   $��brdMain�����#D6D7D6$��1q�
�������._��   � 	Resources     DisabledBackgroundBrush �� Color  WindowBackgroundBrush  HighlightedBrush  NormalBorderBrush �� 
StartPoint �� EndPoint � Color � Offset  NormalBrush  HorizontalNormalBrush  HorizontalNormalBorderBrush  ComboBoxToggleButton �� 
TargetType � ColumnDefinitions ��Name	 F�Name
 �� Triggers P� Property P� Value  ComboBoxTextBox 	 ScrollBarLineButton �� 
TargetType �� Property �� Value �� Path �� RelativeSource �� 
TargetName 
 ScrollBarPageButton  ScrollBarThumb  VerticalScrollBar � RowDefinitions ]� DecreaseRepeatButton ]� Thumb ]� IncreaseRepeatButton  HorizontalScrollBar J� SnapsToDevicePixels �� Padding �� 	ClickMode d� 	IsChecked �� Mode J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate ��Name  �� 	UndoLimit! �� 
IsReadOnly" �� 
IsReadOnly# �� IsDropDownOpen$ � AllowsTransparency% � ActualWidth& �� MaxDropDownHeight' �� Triggers( K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation)   DirectionalNavigation* P� 
SourceName  Contextmenu  DefaultCopyContextmenu+ m� Command  formatTextStyle, �� ElementName  formatImageStyle  formatToggleImageStyle  ToggleContextMenu��%:  &	        &	      &	       &	 3     &	 �     &	 �     &	 5    &	 �    &	 �    &		 *    &	
 !    &	 �    &	 �    7 	 7 
 7  7  7  7 
 7 	 &	 9	    7 	 7 
 7  7  7  7 
 7 	 '�� �
    7  '�� H    7  7  7  7  7   &	     &	 �    7  &	 *    &	 �    &	 �    &	 :    �� $
 #EEE���� $
 #FFF���� $ #CEE7F7���� $	 0,0&�$	 0,1&���� � $
 #CCC��$	 0.0Q�� $
 #444��$	 1.0Q��� $	 0,0&�$	 0,1&���� � $
 #FFF��$	 0.0Q�� $
 #CCC��$	 1.0Q��� $	 0,0&�$	 1,0&���� � $
 #FFF��$	 0.0Q�� $
 #CCC��$	 1.0Q��� $	 0,0&�$	 1,0&���� � $
 #CCC��$	 0.0Q�� $
 #444��$	 1.0Q��� " d�.R��  �� �� $��20 �.+��� $ Border��$��2��
����������#ADAAAD$��1q��� $��0��$��1q�
������ $��0,0,0,0q�F� $	 Arrow��$��1�����#737573$��Center=�$��Center=�"����        � 	=  	= � z     	
 P�  �@J�IsMouseOver . P�  �@d�	IsChecked$
 trueY�P�  � {� .  �� " ��.R��� $ PART_ContentHost��|�.  #��z��$��0q�
������ �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � |� .  ��  � �� �� " ��.R��� $��Border��$��1q�
����������#ADAAAD$��1q�._�F� $��Center=�$��Center=����#737573���� $ Content� �� *TemplatedParent+
 P�  �@J�IsMouseOver . . ��� $ Border�� � ��
 �   �P�  � �� . . ��� $ Border�� � ��
 �   ��� $ Border�� � �� �#F7F3F7P�  � {� .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " ��.R��� ���#F7F3F7�� " n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " n�.R��� $��Border��
����������#ADAAAD$��1q�
 P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#F7F3F7P�  �@n�
IsDragging . . ��� $ Border�� � �� �#F7F3F7�� $ Border�� � ��
 �   �P�  � {� .  �� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����.  �� 8�� $��ScrollBar.PageUpCommand�� n� 8�� $��1,0,1,0q�8�� 8��  �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.   �� 8�� $��ScrollBar.PageLeftCommand�� n� 8�� $��0,1,0,1q�8�� 8��  �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� " ��.���  � �� �#737573��  � ��$ 20����  � ��$ Center=���  � �� �� " ��.R��� $ Bd�� . #��z��#��z��#��z��# z��._��� #��z��#��z��# z 
 P�  �@��IsHighlighted . . ��� 8    � ����  � ��$ 12(�P�  � {� . . ���  � ��$ 12(�P�  � {� .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  � �� �� " ��.R�� .+�d� $��ToggleButton��8��  $��2��|�.  $ Press=� �� $ IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+�� $��ContentSite�� .  #��z #��z #��z��$��3,3,23,3q�$��Center=�$
��Left=��� $ PART_EditableTextBox��$  10��$��LeftToRight=�$	��210��$
��Left=�
������ $	��Top=�$��25��$
��Left=�$	��Top=�$��3,3,23,3q�|�. 
������ $y�Hidden=�#" z! ��Z� � $��Popup��$��Bottom=�#��z# $ . |�.  $��Slide=�.��� $��DropDown�� . #��z% #��z& $
��Auto��.+��� $ DropDownBorder��8�� $��1,0,1,1q����#ADAAAD�� $��4,6,4,6q� .   ���� '��       " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ' P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  .���� ( . $) 	Contained=�
 P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$ 95��P�  � {� .  . ��� $ PART_EditableTextBox��8   � ���� $ ToggleButton�� � {� .  P�  � {� . . ��� $ ToggleButton�� � {� . P�  �@��
IsGrouping . . ���  � �� .  P� $* Popup�� �@�AllowsTransparency . . ��� $ DropDownBorder�� � ��$ 0,0,0,0q�P�  �@��
IsEditable . . ���  � �� .  �� $ PART_EditableTextBox�� � y�$ Visible=��� $ ContentSite�� � y�$ Hidden=�' �� " ��.���  � �� �� " ��.R��� $ 0,3,0,3q����#ADAAAD$
��1.25q�
������� . ._��� �� 8��  .@�m� $��Cut  ��$+ ApplicationCommands.Cut��m� $��Copy  ��$+ ApplicationCommands.Copy��m� $��Paste  ��$+ ApplicationCommands.Paste���� " ��.���  � ��$ Palatino Linotype)���  � ��$ 30����  � ��$ 14(���  � �� �� $, mainRTB���� " ��.���  � ��$ 28����  � �� �� $, mainRTB���� " d�.���  � ��$ 30����  � �� �� $, mainRTB���� " ��.���  � �� �#FFF��  � �� �� " ��.R��� #��z��#��z��#��z��._��� #��z�� . ��  � �� �� " m�.���  � �� �� " m�.R��� #��z��$��Header����.   �� -   - �Name$- 
rowToolbar��$
��Auto ��� -   $- 
rowContent��$��* �.+��� -   $��toolBarTray��$��2q����#D6D7D6$��1q�
�������._��� .+�c� -   $��tbHTMLControls1��$��30��$	��Top=�. `� IsLocked. . 
����������#D6D7D6$	��0.5q�y��� *ShoworHideHTMLControls+.@��� #��[ $��ApplicationCommands.Cut��/ � ToolTip$	/ Cut��y��� *ShoworHideHTMLControls+.���� $G��A/Pointel.Windows.Views.Common.Editor;component/Images/New/Cut.png���� #��[ $��ApplicationCommands.Copy��$
/ Copy��y��� *ShoworHideHTMLControls+.���� $H��B/Pointel.Windows.Views.Common.Editor;component/Images/New/Copy.png���� #��[ $��ApplicationCommands.Paste��$/ Paste��y��� *ShoworHideHTMLControls+.���� $I��C/Pointel.Windows.Views.Common.Editor;component/Images/New/Paste.png���� #��[ $��ApplicationCommands.Undo��$
/ Undo��y��� *ShoworHideHTMLControls+.���� $H��B/Pointel.Windows.Views.Common.Editor;component/Images/New/Undo.png���� #��[ $��ApplicationCommands.Redo��$
/ Redo��y��� *ShoworHideHTMLControls+.���� $H��B/Pointel.Windows.Views.Common.Editor;component/Images/New/Redo.png��d� -   $��
buttonBold��#��[ $ ��EditingCommands.ToggleBold��$
/ Bold��y��� *ShoworHideHTMLControls+.���� $��18(�$
��Bold"�$
��Auto��$��Center=�$��Center=�0 �� TextAlignment$0 Center=����#007edfBd� -   $��buttonItalic��#��[ $"��EditingCommands.ToggleItalic��$/ Italic��y��� *ShoworHideHTMLControls+.���� $��18(�$��Italic$�$
��Bold"�$��20��$��Center=�$��Center=�$0 Center=����#007edfId� -	   $��buttonUnderline��#��[ $%��EditingCommands.ToggleUnderline��$/ 	Underline��y��� *ShoworHideHTMLControls+.���� $��18(�$��	Underline{�$
��Bold"�$��20��$��Center=�$��Center=�$0 Center=����#007edfU�� #��[ $��EditingCommands.AlignLeft��$/ 
Align Left��y��� *ShoworHideHTMLControls+.���� $R��L/Pointel.Windows.Views.Common.Editor;component/Images/New/Left Alignment.png���� #��[ $!��EditingCommands.AlignCenter��$/ Align Center��y��� *ShoworHideHTMLControls+.���� $T��N/Pointel.Windows.Views.Common.Editor;component/Images/New/Center Alignment.png���� #��[ $ ��EditingCommands.AlignRight��$/ Align Right��y��� *ShoworHideHTMLControls+.���� $S��M/Pointel.Windows.Views.Common.Editor;component/Images/New/Right Alignment.png���� #��[ $"��EditingCommands.AlignJustify��$/ Align Justify��y��� *ShoworHideHTMLControls+.���� $K��E/Pointel.Windows.Views.Common.Editor;component/Images/New/Justify.png���� #��[ $&�� EditingCommands.IncreaseFontSize��$/ 	Grow Font��y��� *ShoworHideHTMLControls+.���� $Q��K/Pointel.Windows.Views.Common.Editor;component/Images/New/Increase Font.png���� #��[ $&�� EditingCommands.DecreaseFontSize��$/ Shrink Font��y��� *ShoworHideHTMLControls+.���� $Q��K/Pointel.Windows.Views.Common.Editor;component/Images/New/Decrease Font.png��c� -
   $��tbHTMLControls2��$��32��$	��Top=�. . 
����������#D6D7D6$	��0.5q�.@��� -   $��cmbHTMLFormat��$
��Auto��$��5,0,5,0q�.@��� $
��Text���� $
��HTML���� -   $��cmbFontFamily��$
��Auto��$��75��$��5,0,5,0q�y��� *ShoworHideHTMLControls+�� -   $��cmbFontSize��$��36��y��� *ShoworHideHTMLControls+d� -   1 d�Name$1 ToggleFontColor��#��[ $/ 
Text Color��y��� *ShoworHideHTMLControls+.���� $N��H/Pointel.Windows.Views.Common.Editor;component/Images/New/Text Color.png��2 � ContextMenu2 �� -   3 ��Name$3 FontColorContextMenu��#��[ .@�>   8Pointel.Windows.Views.Common.Editor.Controls.ColorPicker  -   4  Name$4 FontColorPicker��d� -   $1 ToggleLineColor��#��[ $/ 
Back Color��y��� *ShoworHideHTMLControls+.���� $S��M/Pointel.Windows.Views.Common.Editor;component/Images/New/Highlight Color.png��2 �� -   $3 LineColorContextMenu��#��[ .@�  -   $4 LineColorPicker���� -   $��btnInsertHLink��$��32��$/ Add Link��y��� *ShoworHideHTMLControls+.���� $K��E/Pointel.Windows.Views.Common.Editor;component/Images/New/Add URL.png��d� -   $��btnSpellCheck��#��[ $"/ Enable / Disable Spell Check��.���� $N��H/Pointel.Windows.Views.Common.Editor;component/Images/New/SpellCheck.png���� #��[ $)��#EditingCommands.IncreaseIndentation��$/ Increase Indent��y��� *ShoworHideHTMLControls+.���� $S��M/Pointel.Windows.Views.Common.Editor;component/Images/New/Increase Indent.png���� #��[ $)��#EditingCommands.DecreaseIndentation��$/ Decrease Indent��y��� *ShoworHideHTMLControls+.���� $S��M/Pointel.Windows.Views.Common.Editor;component/Images/New/Decrease Indent.png���� #��[ $#��EditingCommands.ToggleBullets��$/ Bullets��y��� *ShoworHideHTMLControls+.���� $M��G/Pointel.Windows.Views.Common.Editor;component/Images/New/Bulleting.png���� #��[ $%��EditingCommands.ToggleNumbering��$/ 	Numbering��y��� *ShoworHideHTMLControls+.���� $M��G/Pointel.Windows.Views.Common.Editor;component/Images/New/Numbering.png���� -   $��btnInsertTable��#��[ $/ Insert Table��$y�	Collapsed=�.���� $E��?/Pointel.Windows.Views.Common.Editor;component/Images/table.png���� -   $��btnInsertImage��#��[ $/ 	Add Image��$y�	Collapsed=�.���� $E��?/Pointel.Windows.Views.Common.Editor;component/Images/Image.png���� -   $��
brdMainRTB��$��1��$��2q����#D6D7D6$��1q�
�������$��Stretch=�$��Stretch=�._�B   <Pointel.Windows.Views.Common.Editor.CustomControls.EditorBox  -   5  Name$5 mainRTB��B   <Pointel.Windows.Views.Common.Editor.CustomControls.EditorBox6   SelectionChanged$6 mainRTB_SelectionChanged��7   PreviewKeyDown$7 mainRTB_PreviewKeyDown��8   MouseLeftButtonDown$8 mainRTB_MouseDown��9   MouseLeftButtonUp$9 mainRTB_MouseUp��:   MouseRightButtonUp$ : mainRTB_MouseRightButtonUp��;   MouseRightButtonDown$"; mainRTB_MouseRightButtonDown��<   TextChanged$< mainRTB_TextChanged��=   KeyUp$= mainRTB_KeyUp��">   PreviewMouseRightButtonDown$)> #mainRTB_PreviewMouseRightButtonDown��? �� IsDocumentEnabled? . $��0q�@ � FocusVisualStyle#@ � )�A   TabIndex$A 0��B �� 
AcceptsTabB . C �� AcceptsReturnC . 
�������"D �� VerticalScrollBarVisibility$
D Auto=�$��Stretch=�$��Stretch=�$E �� HorizontalScrollBarVisibility$
E Auto=�|�. F   OneLineOnlyF �� *OneLineOnly+$, me��    	WavyBrushG j� ViewboxH j� ViewboxUnitsI j� ViewportJ j� ViewportUnitsK j� TileModeL !� Visual��%�  '��       7  'J� �     &	 �     �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ' P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8   �� " J�.���  �@��Margin$ 0q�!� $G 0,0,3,3�$H Absolute=�$I 	0,-5,7,15�$J Absolute=�$
K Tile=�L F� -��������> �@B     �� ��  �0  5 	���	#FFFF0000$	��0.2�� . � -   $��popupAddImage��$��Bottom=�M � 	StaysOpenM .  $ . $��Bottom=�