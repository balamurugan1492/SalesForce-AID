   M S B A M L   `   `   `  ���� \  XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   DisabledBorderBrush  �� 
StartPoint �� EndPoint � Color � Offset  TabItemBrush  	GridBrush  GradiantBrush  GradiantBrush1  
OpticBrush  GradiantBrush2  	DarkBrush  PressedBrush 	 NormalBrush 
 NormalBrush1  HorizontalNormalBrush  
LightBrush  HorizontalLightBrush  NormalBorderBrush  HorizontalNormalBorderBrush  DefaultedBorderBrush  PressedBorderBrush  Tile  
GlassBrush  CBorderBrush 
 CallBG �� Color  ClearBG  NormalBG  DisabledForegroundBrush  DisabledBackgroundBrush  WindowBackgroundBrush  SelectedBackgroundBrush  LightBorderBrush  
GlyphBrush  LightColorBrush  SolidBorderBrush   DisabledBorderBrush1 ! HighlightedBrush " controltempTextbox �� 
TargetType �� Padding ��Name # controltempTextbox1 $ controltempPasswordbox % AnimatedExpanderButtonTemp ��Name	 � ColumnDefinitions
 F�Name J� RenderTransformOrigin �� Angle ��Name �� Triggers P� Property P� Value N� EnterActions �� 
TargetName �� TargetProperty	 W� To g� Duration N� ExitActions �� 
TargetName �� Property �� Value & StretchyExpanderTemp d�Name � OverridesDefaultStyle d� 	IsChecked �� Path �� RelativeSource � LayoutTransform  �� ScaleY ' SimpleExpanderButtonTemp ( SimpleExpanderTemp ) ComboBoxToggleButton * ComboBoxTextBox + ScrollBarLineButton! �� 
TargetType , ScrollBarPageButton - ScrollBarThumb . VerticalScrollBar" � RowDefinitions# ]� DecreaseRepeatButton$ ]� Thumb% ]� IncreaseRepeatButton / HorizontalScrollBar& J� SnapsToDevicePixels' �� 	ClickMode( �� Mode) J� IsHitTestVisible* �� SelectionBoxItem+ �� SelectionBoxItemTemplate, ��Name- �� 	UndoLimit. �� 
IsReadOnly/ �� 
IsReadOnly0 �� IsDropDownOpen1 � AllowsTransparency2 � ActualWidth3 �� MaxDropDownHeight4 � 	Resources5 �� Triggers6 K� IsItemsHost-    'System.Windows.Input.KeyboardNavigation7    DirectionalNavigation8 P� 
SourceName 0 CloseButton9 ��Name: �� Triggers; �� Value< �� Binding= �� Mode> �� AncestorType? � 	Resources 1 Key@ g� 	BeginTimeA O� KeyTimeB O� ValueC �� ScaleXD �� AngleXE �� AngleYF T� XG T� YH :� RoutedEvent 2 OnMouseEnter1 3 MinimizeButton 4 	PinButton 5 TCStackPannelImageButton 6 TCArrowImageButton 7 NormalImageButton 8 
CallButtonI �� CornerRadiusJ W� FromK g� AutoReverse 9 RejectButton : ClearButton ; NormalButton1 < NormalButton = CheckBoxFocusVisualL �� StrokeDashArrayM �� 
BackgroundN �� Bullet > RadioButtonFocusVisualO A�NameP K� ZIndex ? ContextmenuQ m� IsSubmenuOpenR z� SharedSizeGroupS ��NameT m� InputGestureText @ MenuitemU m� SubmenuHeaderTemplateKeyV m� SubmenuItemTemplateKey W m� TopLevelHeaderTemplateKeyX m� TopLevelItemTemplateKey�� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     4my-http://schemas.microsoft.com/wpf/2008/toolkit  :x,http://schemas.microsoft.com/winfx/2006/xaml      %�W  &	        &	 �     &	 A    &	 �    &	      &	 n    &	 �    &	     &	 p    &		 �    &	
 P    &	 �    &	 �    &	 S    &	 �    &	 �    &	 U    &	 �    &	     &	 w    &	 �    &	 D    &	 W    &	 j    &	 }    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	  �    &	! 	    &	"  	    &	# �	    &	$ 
    &	% s
    &	& �    7 % &	'     &	( �    7 ' &	) �    &	* <    &	+ �    &	, }    &	- 6    &	. �    7 + 7 , 7 - 7  7  7 , 7 + &	/ �    7 + 7 , 7 - 7 	 7  7 , 7 + '�� M    7 ! '�� �    7 ) 7  7 / 7 . 7  &	0 t%    7 2 &	3 ~*    7 2 &	4 �/    7 2 &	5 �2    &	6 �3    &	7 �4    &	8 �5    &	9 7    &	: �7    &	; Z9    &	< �:    '�� d<    &	= =    '�� p=    7 = 7  7  &	> �@    '	� YA    7 > 7  '�� LD    '�� E    '�� �F    &	? 6G    (�� �G    *$"MenuItem.TopLevelHeaderTemplateKey+)7 ! 7  7  (�� HJ    *" MenuItem.TopLevelItemTemplateKey+)7 ! 7  (�� /K    *!MenuItem.SubmenuItemTemplateKey+)7 	 7  7  7 ! 7  (�� �N    *#!MenuItem.SubmenuHeaderTemplateKey+)7  7  7 ! 7  7  &	@ 	R    7U 7V 7W 7X �� $	  0,0&�$	 0,1&���� � $ Orange��$ 0Q�� $ #C0C0C1��$
 0.25Q�� $ #C0C0C0��$
 0.75Q�� $ Orange��$ 1Q��� $	  1,0&�$	 0,1&���� � $ Orange��$ 0Q�� $ #ffc14e��$
 0.25Q�� $ #ffcf76��$	 0.5Q�� $ #ffb327��$
 0.75Q�� $ Orange��$ 1Q��� $	  0,0&�$	 0,1&���� � $
 #FFF��$ 1Q�� $
 Gray��$	 0.5Q�� $
 Gray��$ 0Q��� $	  0,0&�$	 0,1&���� � $
 #FFF��$ 0Q�� $ Black��$ 1Q��� $	  0,0&�$	 0,1&���� � $
 #FFF��$ 0Q�� $
 Gray��$	 0.9Q�� $ Black��$ -1Q��� $	  1,0&�$	 0,1&���� � $ Black��$	 1.0Q�� $
 #AAA��$	 0.0Q��� $	  0,0&�$	 0,1&���� � $
 #FFF��$ 0Q�� $ #B6B6B6��$ 1Q��� $	  0,0&�$	 0,1&���� � $
 #FFF��$	 0.0Q�� $
 #AAA��$	 1.0Q��� $	  0,0&�$	 0,1&���� � $
 #BBB��$	 0.0Q�� $
 #EEE��$	 0.1Q�� $
 #EEE��$	 0.9Q�� $
 #FFF��$	 1.0Q��� $	  0,0&�$	 0,1&���� � $
 #FFF��$	 0.0Q�� $
 #CCC��$	 1.0Q��� $	  0,0&�$	 0,1&���� � $
 #FFF��$	 0.0Q�� $ Black��$	 1.0Q��� $	  0,0&�$	 1,0&���� � $
 #FFF��$	 0.0Q�� $
 #CCC��$	 1.0Q��� $	  0,0&�$	 0,1&���� � $
 #FFF��$	 0.0Q�� $
 #EEE��$	 1.0Q��� $	  0,0&�$	 1,0&���� � $
 #FFF��$	 0.0Q�� $
 #EEE��$	 1.0Q��� $	  0,0&�$	 0,1&���� � $
 #CCC��$	 0.0Q�� $
 #444��$	 1.0Q��� $	  0,0&�$	 1,0&���� � $
 #CCC��$	 0.0Q�� $
 #444��$	 1.0Q��� $	  0,0&�$	 0,1&���� � $
 #777��$	 0.0Q�� $
 #000��$	 1.0Q��� $	  0,0&�$	 0,1&���� � $
 #444��$	 0.0Q�� $
 #888��$	 1.0Q��� $	  1,0&�$	 0,1&���� � $ White��$ 0Q�� $ 	AliceBlue��$
 0.55Q�� $ #808080��$ 1Q��� $	  0,0&�$	 1,0&�.��� $ 0Q�$ 	#66ffffff��� $ 0Q�$ 	#36FFFFFF��� $ 1Q�$ 	#B5EBE7E7���� $	  0,0&�$	 0,1&���� � $ #707f8b��$ 1Q�� $ #c6cdd1��$	 0.1Q��� $ #358535���� $ #b28e00���� $ #0061ac���� $
 #888���� $
 #EEE���� $
 #FFF���� $
 #DDD���� $
 #AAA���� $
 #444���� $
 #DDD���� $
 #888���� $
 #AAA���� $ #CEE7F7���� " ��.R��� $ 2q�
����������#ADAAAD$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " ��.R��� $ 2q�
����������#ADAAAD$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " G�.R��� $ 2q�
����������#888588$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " d�.R��� $ ExpanderButtonBorder��#��z��#��z��#��z��# z��._�� $	��Top=�	 �� $
��Auto ��� $��* �.+��� 
������ $��2��A� $��Circle��$��0��
�������$��20��$��20��$��Center=�$��Center=�F� $
 Arrow��$��0��"����@B `� � �D @KL � z `� 	���	#FF666666$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $ 0Q��� $ HeaderContent��$��1��$��4,0,0,0q�$��Center=�$��Content�� P�  �@d�	IsChecked$
 TrueY� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$	 180X�$ 0:0:0.4D� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$ 0X�$ 0:0:0.4D�P�  �@J�IsMouseOver . . ��� $ Circle�� � �� �#808080P�  � �� . . ��� $ Circle�� � �� �#808080�� $ Arrow�� � ��
 ������� " 9�.R�]� .+�d� $ ExpanderButton��$��35��$	��Top=����#808080$��0,0,0,1q�8��  #��z�� . $��1.5,0q� �� $ 
IsExpanded� �� *TemplatedParent+�� $ ExpanderContent��$��Content��$��Bottom=� �� $  0Q� P�  �@9�
IsExpanded .  �� .���� .��W� $ ExpanderContent��$@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�$ 1X�$ 0:0:0.4D� �� .���� .��W� $ ExpanderContent��$@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�$ 0X�$ 0:0:0.4D��� " d�.R��� $ ExpanderButtonBorder��#��z��#��z��#��z��# z��._�� 	 �� $
��Auto ��� $��* �.+��� 
������ $��2��A� $��Circle��$��0��
�������$��20��$��20��$��Center=�$��Center=�F� $
 Arrow��$��0��"����@B `� � �D @KL � z `� 	
�������$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $ 0Q��� $ HeaderContent��$��1��$��4,0,0,0q�$��Content�� P�  �@d�	IsChecked$
 TrueY� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$	 180X�$ 0:0:0.4D� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$ 0X�$ 0:0:0.4D��� " 9�.R�]� .+�d� $ ExpanderButton��$	��Top=�8��  #��z�� . $��1.5,0q� �� $ 
IsExpanded� �� *TemplatedParent+�� $ ExpanderContent��$��1��$y�	Collapsed=�$��Bottom=� P�  �@9�
IsExpanded . . ��� $ ExpanderContent�� � y�$ Visible=��� " d�.R�� 	 �� �� $��20 �.+��� $ Border��$��2��
����������#ADAAAD$��1q��� $��0��$��1q�
������ $��0,0,0,0q�F� $
 Arrow��$��1�����#737573$��Center=�$��Center=�"����        � 	=  	= � z     	 P�  �@J�IsMouseOver . P�  �@d�	IsChecked$
 trueY�P�  � {� .  �� " ��.R��� $ PART_ContentHost��|�.  #��z��$��0q�
������ �� "! ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � |� .  ��  � �� �� " ��.R��� $��Border��$��1q�
����������#ADAAAD$��1q�._�F� $��Center=�$��Center=����#737573���� $ Content� �� *TemplatedParent+ P�  �@J�IsMouseOver . . ��� $ Border�� � ��
 �   �P�  � �� . . ��� $ Border�� � ��
 �   ��� $ Border�� � �� �#F7F3F7P�  � {� .  �� "! ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " ��.R��� ���#F7F3F7�� "! n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " n�.R��� $��Border��
����������#ADAAAD$��1q� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#F7F3F7P�  �@n�
IsDragging . . ��� $ Border�� � �� �#F7F3F7�� $ Border�� � ��
 �   �P�  � {� .  �� " ��.R�� " �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����. # �� 8�� $��ScrollBar.PageUpCommand��$ n� 8�� $��1,0,1,0q�8�� 8�� % �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� " ��.R�� 	 �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.  # �� 8�� $��ScrollBar.PageLeftCommand��$ n� 8�� $��0,1,0,1q�8�� 8�� % �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� "! ��.���  � �� �#737573��  � ��$ 20����  � ��$ Center=���  � �� �� " ��.R��� $ Bd��& . #��z��#��z��#��z��# z��._��� #��z��#��z��#& z&  P�  �@��IsHighlighted . . ��� 8    � ����  � ��$ 12(�P�  � {� . . ���  � ��$ 12(�P�  � {� .  �� "! ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  � �� �� " ��.R�� .+�d� $��ToggleButton��8��  |�.  $' Press=� �� $ IsDropDownOpen�$( TwoWay=� �� *TemplatedParent+�� $��ContentSite��) .  #��z* #��z+ #��z��$��3,3,23,3q�$��Center=�$
��Left=��� $, PART_EditableTextBox��$- 10��$��LeftToRight=�$	��210��$
��Left=�
������ $��0q�$	��Top=�$��25��$
��Left=�$	��Top=�$��3,3,23,3q�|�. 
������ $y�Hidden=�#/ z. ��Z� � $��Popup��$��Bottom=�#��z0 1 . |�.  $��Slide=�.��� $��DropDown��& . #��z2 #��z3 $
��Auto��.+��� $ DropDownBorder��8�� $��1,0,1,1q����#ADAAAD�� $��4,6,4,6q�& . 4 ���� '��       "! ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . 5 P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  .���� 6 . $7 	Contained=� P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$ 95��P�  � {� .  . ��� $ PART_EditableTextBox��8   � ���� $ ToggleButton�� � {� .  P�  � {� . . ��� $ ToggleButton�� � {� . P�  �@��
IsGrouping . . ���  � �� .  P� $8 Popup�� �@�AllowsTransparency . . ��� $ DropDownBorder�� � ��$ 0,0,0,0q�P�  �@��
IsEditable . . ���  � �� .  �� $ PART_EditableTextBox�� � y�$ Visible=��� $ ContentSite�� � y�$ Hidden=�5 �� "! ��.���  � �� �� .R��� $9 Close��$B��</Agent.Interaction.Desktop;component/Images/Custom Close.png��$��10��$��10��: �� $
; True��< ��  �� $= FindAncestor=�"> ��$ IsMouseOver�.a��� $ Close�� � ��$K E/Agent.Interaction.Desktop;component/Images/Custom Close selected.png���� $
; True��< ��  �� $= FindAncestor=�"> ��$ 	IsPressed�.a��� $ Close�� � ��$K E/Agent.Interaction.Desktop;component/Images/Custom Close selected.png����  � �� �� " ��? ���� OnMouseEnter11 .��U� $@ 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $A 00:00:00��$	B 1.4Q��� $A 00:00:01��$B 1Q�U� $@ 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $A 00:00:00��$	B 1.4Q��� $A 00:00:01��$B 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z��z�V� .~��� $C 1Q�$  1Q��� $D 0Q�$E 0Q��� $ 0Q�T� $F 0Q�$G 0Q� :� $H Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� "! ��.���  � �� �� .R��� $9 Minimize��$E��?/Agent.Interaction.Desktop;component/Images/Custom Minimize.png��$��15��$��11��: �� $
; True��< ��  �� $= FindAncestor=�"> ��$ IsMouseOver�.a��� $ Minimize�� � ��$N H/Agent.Interaction.Desktop;component/Images/Custom Minimize selected.png���� $
; True��< ��  �� $= FindAncestor=�"> ��$ 	IsPressed�.a��� $ Minimize�� � ��$N H/Agent.Interaction.Desktop;component/Images/Custom Minimize selected.png����  � �� �� " ��? ���� OnMouseEnter11 .��U� $@ 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $A 00:00:00��$	B 1.4Q��� $A 00:00:01��$B 1Q�U� $@ 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $A 00:00:00��$	B 1.4Q��� $A 00:00:01��$B 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z��z�V� .~��� $C 1Q�$  1Q��� $D 0Q�$E 0Q��� $ 0Q�T� $F 0Q�$G 0Q� :� $H Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� "! ��.���  � �� �� " ��? ���� OnMouseEnter11 .��U� $@ 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $A 00:00:00��$	B 1.4Q��� $A 00:00:01��$B 1Q�U� $@ 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $A 00:00:00��$	B 1.4Q��� $A 00:00:01��$B 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z��z�V� .~��� $C 1Q�$  1Q��� $D 0Q�$E 0Q��� $ 0Q�T� $F 0Q�$G 0Q� :� $H Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� "! ��5 �� < ��  �� *Self+$ IsKeyboardFocusWithin�; True.a���  � ��
 ���� �� < ��  �� *Self+$ IsMouseOver�; True.a���  � �� �#d6ebff�� "! ��.���  � �� �� " ��.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � ��
 ���� P�  � �� . . ��� $ Border�� � �� �#94c3e7�� "! ��.���  � �� �� " ��.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� "! ��.���  � �� �� " ��.R��� $��Border��$I 2�����#4DBF4D
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #& z& #��z�� :� $H Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$J 1X�$
 0.25X�$ 0:0:0.25D�K . P�  � �� . . ��� $ Border�� � �� �#358535�� "! ��.���  � �� �� " ��.R��� $��Border��$I 2�����#f94e4b
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #& z& #��z�� P�  � �� . . ��� $ Border�� � �� �#EF0C08�� "! ��.���  � �� �� " ��.R��� $��Border��$I 2�����#FFCC00
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #& z& #��z�� :� $H Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$J 1X�$
 0.25X�$ 0:0:0.25D�K . P�  � �� . . ��� $ Border�� � �� �#b28e00�� "! ��.���  � �� �� " ��.R��� $ Border��$I 0,0,2,2��
������ 
������ $��1,0,1,1q�._�F� $
 Arrow��
�������$��Center=�$��Bottom=�"����        � 	=  	= � z     	 P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#007edfP�  � �� . . ��� $ Border�� � �� �#0061ac�� "! ��.���  � �� �� " ��.R��� $��Border��$I 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #& z& #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#007edf:� $H Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$J 1X�$
 0.25X�$ 0:0:0.25D�K . P�  � �� . . ��� $ Border�� � �� �#0061ac�� "! ��.���  � ��
 �   ���  � ��$ Calibri)���  � ��$ 12(���  � �� �� " ��.R��� #��z��._��� $��5q�#��z��#��z��#& z& ��.  �� .���  � �� �� .R��� ._��� $��15,0,0,0q�$��1�����	#60000000$	L 2 2R��� "! ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  �@�FocusVisualStyle8   ��  � �� �� " ��.R��� $��Center=�
M ���� N �� $ Border��$��13��$��13��$I 0��
�������$��1q����#ADAAAD._�F� $
 	CheckMark��$��7��$��7��& .  8�� $��2��+����        ���j ��j �    ��j ���j     	._��� $��4,0,0,0q�$��Center=�$
��Left=���.  P�  �@d�	IsChecked$ falseY�. ��� $ 	CheckMark�� � y�$ 	Collapsed=�P�  Z�  �@d�	IsChecked. ��� $ 	CheckMark�� � �� ��    ��j ���j     	P�  �@J�IsMouseOver . . ��� $ Border�� � ��
 �   �P�  � �� . . ��� $ Border�� � �� �#F7F3F7�� $ Border�� � ��
 �   �P�  � {� .  . ��� $ Border��8   � ���� $ Border�� � �� �	#40000000��  � �� �	#80000000�� .���  � �� �� .R��� ._��� $��15,0,0,0q�$��1�����	#60000000$	L 1 2R��� "! 	�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  �@�FocusVisualStyle8   ��  � �� �� " 	�.R��� $��Center=�
M ���� N � $��13��$��13��.+�A� $O Border��
�������$��1�����#ADAAADA� $O 	CheckMark��$��4q�
���   �._��� $��4,0,0,0q�$��Center=�$
��Left=���.  P�  �@d�	IsChecked$ falseY�. ��� $ 	CheckMark�� � y�$ 	Collapsed=�P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#F7F3F7P�  � �� . . ��� $ Border�� � �� �#F7F3F7�� $ Border�� � ��
 �   �P�  � {� .  . ��� $ Border��8   � ���� $ Border�� � �� �	#40000000��  � �� �	#80000000�� "! ��.���  � �� �� " ��.R�� " �� $
��Auto ��� $��* �.+��� $��0��$P 1��$��0,0,0,0q�6 . 
��������� $��1�����#D6D7D6$��1q�._��� $��SelectedContent���� "! ��.���  � �� �� " ��.R�� .+��� $��Border��
�������$��1,1,1,0q�
������ $��Calibri)�$��12(�._��� $ ContentSite��$��Center=�$��Center=�$��Header��$��20����. $��5,5,5,0q� P�  �@��
IsSelected . . ��� $ Border�� � �� �#0071C6�� $ Border�� � �� �#D6D7D6�� $ Border�� � ��$ 0,0,0,-2q��� $ ContentSite�� � ��$ 5,2,5,0q��� "! ��.���  � �� �� " ��.R��� $ 0,3,0,3q����#ADAAAD$
��1.25q�
�������& . ._��� �� "! ��.���  � �� �� " ��.R��� $ 0,3,0,3q����#ADAAAD$
��1.25q�
�������& . ._��� �� " m�.R��� $��Border��._�� .+��� $��	-10,3,6,3q�$��Header����. � $��Popup��$��Bottom=�#��zQ 1 . |�.  $
��Fade=�.���� $��SubmenuBorder��& . ���#FFFFFF���#888888$��1q�._��� 6 . $7 Cycle=� P� " �@m�IsSuspendingPopupAnimation . . ��� $ Popup�� � ��$
 None=�P�  �@m�IsHighlighted . . ��� $ Border��8    � ���� $ Border�� � ��
 ���� P� $8 Popup�� �@�AllowsTransparency . . ��� $ SubmenuBorder�� �@��Padding$ 0,3,0,3q��� $ SubmenuBorder��8   � ���� $ SubmenuBorder�� � ��$
 1.25q�P�  � {� .  . ��� 8   � ���� " m�.R��� $��Border��._�� .+��� $��6,3,6,3q�$��Header����.  P�  �@m�IsHighlighted . . ��� $ Border��8    � ���� $ Border�� � ��
 ���� P�  � {� .  . ��� 8   � ���� " m�.R��� $��Border��$ 0,3,0,3q�._�� 	 �� $
��Auto �$
R Icon���� $��* ��� $
��Auto �$R Shortcut���� $��13 �.+��� $
��Icon��$��6,0,6,0q�$��Center=�$
��Icon���� $��Check��$��13��$��13��$y�	Collapsed=�8��  $��1q�8�� ._�F� $��	CheckMark��$��7��$��7��$y�Hidden=�& .  8�� $��2��+����        ���j ��j �    ��j ���j     	�� $��
HeaderHost��$��1��$��Header����. �� $S InputGestureText��$��2��#��zT $��2,0,2,0q�$��Right=� P�  Z�  �@m�Icon. ��� $
 Icon�� � y�$ Hidden=�P�  �@m�	IsChecked . . ��� $ 	CheckMark�� � y�$ Visible=�P�  �@m�IsCheckable . . ��� $ Check�� � y�$ Visible=��� $
 Icon�� � y�$ Hidden=�P�  �@m�IsHighlighted . . ��� $ Border��8   � ��P�  � {� .  . ��� 8   � ���� " m�.R��� $��Border��$ 0,3,0,3q�._�� 	 �� $
��Auto �$
R Icon���� $��* ��� $
��Auto �$R Shortcut���� $��10 �.+��� $
��Icon��$��Center=�$
��Icon���� $��
HeaderHost��$��1��$��Header����. �� $S InputGestureText��$��2��#��zT $��2,0,2,0q�$��Right=�F� $��3��$��Center=�$��Center=�"����        �    ��j � 	= �g5 	8��  � $��Popup��$��Right=�#��zQ 1 . |�.  $��Slide=�.���� $��SubmenuBorder��& . 8�� ._��� 6 . $7 Cycle=� P�  Z�  �@m�Icon. ��� $
 Icon�� � y�$ Hidden=�P�  �@m�IsHighlighted . . ��� $ Border��8   � ��P� $8 Popup�� �@�AllowsTransparency . . ��� $ SubmenuBorder�� �@��Padding$ 0,3,0,3q��� $ SubmenuBorder��8   � ���� $ SubmenuBorder�� � ��$
 1.25q�P�  � {� .  . ��� 8   � ���� "! m�.���  �@�OverridesDefaultStyle . 5 P�  �@m�Role$ SubmenuHeader=�. ���  � ��8   P�  �@m�Role$ SubmenuItem=�. ���  � ��8  P�  �@m�Role$ TopLevelHeader=�. ���  � ��8  ��  �@�IsSharedSizeScope . P�  �@m�Role$ TopLevelItem=�. ���  � ��8  