   M S B A M L   `   `   `  ���� X  TPointel.Interactions.Outbound, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullk:clr-namespace:Pointel.Interactions.Outbound.CustomControls,Pointel.Interactions.Outbound.CustomControls  p lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   DisabledBorderBrush  �� 
StartPoint �� EndPoint � Color � Offset  TabItemBrush  	GridBrush  GradiantBrush  GradiantBrush1  
OpticBrush  GradiantBrush2  	DarkBrush  PressedBrush 	 NormalBrush 
 NormalBrush1  HorizontalNormalBrush  
LightBrush  HorizontalLightBrush  NormalBorderBrush  HorizontalNormalBorderBrush  DefaultedBorderBrush  PressedBorderBrush  Tile  
GlassBrush  CBorderBrush % !HorizontalSliderTrackNormalBorder ) %HorizontalSliderTrackNormalBackground �� Color 
 CallBG  ClearBG  NormalBG  DisabledForegroundBrush  DisabledBackgroundBrush  WindowBackgroundBrush  SelectedBackgroundBrush  LightBorderBrush  
GlyphBrush   LightColorBrush ! SolidBorderBrush " DisabledBorderBrush1 # HighlightedBrush $ controltempTextbox �� 
TargetType �� Padding ��Name % controltempTextbox1 & MyWaterMarkStyle �� 
TargetType	 �� Property
 �� Value ��Name
 � Tag �� Triggers ]� 
Conditions �� Property �� Value �� 
TargetName P� Property P� Value ' controltempPasswordbox ( AnimatedExpanderButtonTemp ��Name � ColumnDefinitions F�Name J� RenderTransformOrigin �� Angle ��Name N� EnterActions �� 
TargetName �� TargetProperty	 W� To g� Duration N� ExitActions ) StretchyExpanderTemp  d�Name! � OverridesDefaultStyle" d� 	IsChecked# �� Path$ �� RelativeSource% � LayoutTransform& �� ScaleY * SimpleExpanderButtonTemp + SimpleExpanderTemp , ComboBoxToggleButton - ComboBoxTextBox . ScrollBarLineButton / ScrollBarPageButton 0 ScrollBarThumb 1 VerticalScrollBar' � RowDefinitions( ]� DecreaseRepeatButton) ]� Thumb* ]� IncreaseRepeatButton 2 HorizontalScrollBar+ J� SnapsToDevicePixels, �� 	ClickMode- �� Mode. J� IsHitTestVisible/ �� SelectionBoxItem0 �� SelectionBoxItemTemplate1 ��Name2 �� 	UndoLimit3 �� 
IsReadOnly4 �� 
IsReadOnly5 �� IsDropDownOpen6 � AllowsTransparency7 � ActualWidth8 �� MaxDropDownHeight9 � 	Resources: �� Triggers; K� IsItemsHost-   'System.Windows.Input.KeyboardNavigation<    DirectionalNavigation= P� 
SourceName 3 CloseButton> ��Name? �� Triggers@ �� ValueA �� BindingB �� ModeC �� AncestorTypeD � 	Resources 4 KeyE g� 	BeginTimeF O� KeyTimeG O� ValueH �� ScaleXI �� AngleXJ �� AngleYK T� XL T� YM :� RoutedEvent 5 OnMouseEnter1 6 numberButton 7 numberButton1N �� CornerRadius 8 MinimizeButton 9 	PinButton : TCStackPannelImageButton ; TCArrowImageButton < NormalImageButton = NormalImageButton1 > 
CallButtonO W� FromP g� AutoReverse ? RejectButton @ ClearButton A NormalButton1 B NormalButton C ChatSendButton D 
SideButton=  7Pointel.Interactions.Outbound.CustomControls.TimerLabelQ   Text E CheckBoxFocusVisualR �� StrokeDashArrayS �� 
BackgroundT �� BulletU K� ZIndex F ContextmenuV m� IsSubmenuOpenW z� SharedSizeGroupX ��NameY m� InputGestureText G MenuitemZ m� SubmenuHeaderTemplateKey[ m� SubmenuItemTemplateKey \ m� TopLevelHeaderTemplateKey] m� TopLevelItemTemplateKey)  #Microsoft.Windows.Controls.DataGrid^   SelectAllCommand_ �� TypeInTargetAssembly` �� 
ResourceId!a   HeadersVisibilityConverterb �� 	Converter:  4Microsoft.Windows.Controls.DataGridHeadersVisibility
c   Alld �� ConverterParameterJ  DMicrosoft.Windows.Controls.Primitives.DataGridColumnHeadersPresentere   Columnf ��Nameg �� CanContentScrollh �� Orientationi �� ScrollableHeightj � Maximumk �� ViewportHeightl �� ViewportSize*m �� #ComputedVerticalScrollBarVisibilityn � Valueo �� ScrollableWidthp �� ViewportWidth,q �� %ComputedHorizontalScrollBarVisibility-  'Microsoft.Windows.Controls.DataGridCell H ColumnHeaderGripperStyle@  :Microsoft.Windows.Controls.Primitives.DataGridColumnHeaderr �� ContentStringFormats �� ContentStringFormatt E� 
StartPointu �� Pointv n�Name=  7Microsoft.Windows.Controls.Primitives.DataGridRowHeader I maximizeButton J RestoreButton K alternatingStyle L OrangeHeaderTemplate M SliderRepeatButtonStyle N CustomThumbForSlider O MyCustomStyleForSliderw m�Namex m� Filly m� 	Placementz ��Name{ ]�Name| �� DecreaseLarge} �� IncreaseLarge P CustomHeaderStyle�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     6my-http://schemas.microsoft.com/wpf/2008/toolkit  Dlocal:clr-namespace:Pointel.Interactions.Outbound.CustomControls  ]autoTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit  :x,http://schemas.microsoft.com/winfx/2006/xaml     %�  &	        &	 �     &	 A    &	 �    &	      &	 n    &	 �    &	     &	 p    &		 �    &	
 P    &	 �    &	 �    &	 S    &	 �    &	 �    &	 U    &	 �    &	     &	 w    &	 �    &	 D    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	 	    &	 	    &	 %	    &	 5	    &	  E	    &	! U	    &	" e	    &	# u	    &	$ �	    &	% �	    &	& f
    &	' �    &	( b    &	) �    7 ( &	*     &	+ �    7 * &	, �    &	- J    &	. �    &	/ x    &	0 1    &	1 �    7 . 7 / 7 0 7  7  7 / 7 . &	2 �    7 . 7 / 7 0 7 	 7  7 / 7 . '�� H    7 # '�� �     7 , 7  7 2 7 1 7  &	3 o'    7 5 &	6 y,    &	7 �.    &	8 �0    7 5 &	9 �5    7 5 &	: �8    &	; �9    &	< �:    &	= �;    &	> �<    &	? V>    &	@ I?    &	A �@    &	B B    &	C �C    &	D eE    7  '�� �F    '  JG    &	E �G    '��  H    7 E 7  7  '�� �K    '�� tL    '�� !N    &	F �N    (�� O    *$"MenuItem.TopLevelHeaderTemplateKey+)7 # 7  7  (�� �Q    *" MenuItem.TopLevelItemTemplateKey+)7 # 7  (�� �R    *!MenuItem.SubmenuItemTemplateKey+)7 	 7  7  7 # 7  (�� �U    *#!MenuItem.SubmenuHeaderTemplateKey+)7  7  7 # 7  7  &	G fY    7Z 7[ 7\ 7] '  �Z    7 1 7 2 '  �`    &	H %a    '  �a    '  �a    7 H 7 H '  �g    7 	 &	I �h    7 5 &	J �m    7 5 &	K �r    &	L qs    &	M �s    &	N Ft    &	O �t    7  7  7 M 7 M 7 N &	P w    7 H 7 H �� $	  0,0&�$	 0,1&���� � $ Orange��$ 0Q�� $ #C0C0C1��$
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
 0.55Q�� $ #808080��$ 1Q��� $	  0,0&�$	 1,0&�.��� $ 0Q�$ 	#66ffffff��� $ 0Q�$ 	#36FFFFFF��� $ 1Q�$ 	#B5EBE7E7���� $	  0,0&�$	 0,1&���� � $ #707f8b��$ 1Q�� $ #c6cdd1��$	 0.1Q��� $	 0,1&�$	  0,0&�.��� $ 	#FFAEB1AF��$	 0.1Q�� $ White��$ .9Q��� $ #DEDEDE���� $ #358535���� $ #b28e00���� $ #0061ac���� $
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
�������
������ $��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " ��.��� 	 � ��
 �� " ��.R�� .+��� 
�������
������ $��1q��� $ PART_ContentHost��$��5,0,0,0q�$��Center=��� $ WaterMarkLabel��$��5,0,0,0q�#��z $��Center=�$y�	Collapsed=�
�������$��Calibri)�$��12(�$��Italic$� ]�  ��  � ��$  ��./��� $ WaterMarkLabel��	 � y�$
 Visible=�P�  � {� .  . ��� 	 � ��

 �iii��� " G�.R��� $ 2q�
����������#ADAAAD$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " d�.R��� $ ExpanderButtonBorder��#��z��#��z��#��z��# z��._�� $	��Top=� �� $
��Auto ��� $��* �.+��� 
������ $��2��A� $��Circle��$��0��
�������$��20��$��20��$��Center=�$��Center=�F� $ Arrow��$��0��"����@B `� � �D @KL � z `� 	���	#FF666666$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $ 0Q��� $ HeaderContent��$��1��$��4,0,0,0q�$��Center=�$��Content�� P�  �@d�	IsChecked$
 TrueY� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$	 180X�$ 0:0:0.4D� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$ 0X�$ 0:0:0.4D�P�  �@J�IsMouseOver . . ��� $ Circle��	 � ��
 �#808080P�  � �� . . ��� $ Circle��	 � ��
 �#808080�� $ Arrow��	 � ��

 ������� " 9�.R�]� .+�d� $  ExpanderButton��$��35��$	��Top=����#808080$��0,0,0,1q�8��  #��z��! . $��1.5,0q�" �� $# 
IsExpanded�$ �� *TemplatedParent+�� $ ExpanderContent��$��Content��$��Bottom=�% �� $& 0Q� P�  �@9�
IsExpanded .  �� .���� .��W� $ ExpanderContent��$@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�$ 1X�$ 0:0:0.4D� �� .���� .��W� $ ExpanderContent��$@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�$ 0X�$ 0:0:0.4D��� " d�.R��� $ ExpanderButtonBorder��#��z��#��z��#��z��# z��._��  �� $
��Auto ��� $��* �.+��� 
������ $��2��A� $��Circle��$��0��
�������$��20��$��20��$��Center=�$��Center=�F� $ Arrow��$��0��"����@B `� � �D @KL � z `� 	
�������$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $ 0Q��� $ HeaderContent��$��1��$��4,0,0,0q�$��Content�� P�  �@d�	IsChecked$
 TrueY� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$	 180X�$ 0:0:0.4D� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$ 0X�$ 0:0:0.4D��� " 9�.R��� ���#FEC732$��1q�._�]� .+�d� $  ExpanderButton��$	��Top=�8��  #��z��! . $��1.5,0q�" �� $# 
IsExpanded�$ �� *TemplatedParent+�� $ ExpanderContent��$��1��$y�	Collapsed=�$��Bottom=� P�  �@9�
IsExpanded . . ��� $ ExpanderContent��	 � y�$
 Visible=��� " d�.R��  �� �� $��20 �.+��� $ Border��$��2��
����������#ADAAAD$��1q��� $��0��$��1q�
������ $��0,0,0,0q�F� $ Arrow��$��1�����#737573$��Center=�$��Center=�"����        � 	=  	= � z     	 P�  �@J�IsMouseOver . P�  �@d�	IsChecked$
 trueY�P�  � {� .  �� " ��.R��� $ PART_ContentHost��|�.  #��z���� " ��.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . �� 	 � |�
 .  �� 	 � ��
 �� " ��.R��� $��Border��$��1q�
����������#ADAAAD$��1q�._�F� $��Center=�$��Center=����#737573���� $# Content�$ �� *TemplatedParent+ P�  �@J�IsMouseOver . . ��� $ Border��	 � ��

 �   �P�  � �� . . ��� $ Border��	 � ��

 �   ��� $ Border��	 � ��
 �#F7F3F7P�  � {� .  �� " ��.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . �� 	 � ��
 .  �� 	 � |�
 .  �� 	 � ��
 �� " ��.R��� ���#F7F3F7�� " n�.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . �� 	 � ��
 .  �� 	 � |�
 .  �� 	 � ��
 �� " n�.R��� $��Border��
����������#ADAAAD$��1q� P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#F7F3F7P�  �@n�
IsDragging . . ��� $ Border��	 � ��
 �#F7F3F7�� $ Border��	 � ��

 �   �P�  � {� .  �� " ��.R�� ' �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����. ( �� 8�� $��ScrollBar.PageUpCommand��) n� 8�� $��1,0,1,0q�8�� 8�� * �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.  ( �� 8�� $��ScrollBar.PageLeftCommand��) n� 8�� $��0,1,0,1q�8�� 8�� * �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� " ��.��� 	 � ��
 �#737573�� 	 � ��$
 20���� 	 � ��$
 Center=��� 	 � ��
 �� " ��.R��� $ Bd��+ . #��z��#��z��#��z��# z��._��� #��z��#��z��#+ z+  P�  �@��IsHighlighted . . ��� 8
   	 � ���� 	 � ��$
 12(�P�  � {� . . ��� 	 � ��$
 12(�P�  � {� .  �� " ��.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . �� 	 � ��$

 Auto=��� 	 � ��$

 Auto=��� 	 � ��
 . �� 	 � ��
 �� " ��.R�� .+�d� $��ToggleButton��8��  $��2��|�.  $, Press=�" �� $# IsDropDownOpen�$- TwoWay=�$ �� *TemplatedParent+�� $��ContentSite��. .  #��z/ #��z0 #��z��$��3,3,23,3q�$��Center=�$
��Left=��� $1 PART_EditableTextBox��$2 10��$��LeftToRight=�$	��210��$
��Left=�
������ $	��Top=�$��25��$
��Left=�$	��Top=�$��3,3,23,3q�|�. 
������ $y�Hidden=�#4 z3 ��Z� � $��Popup��$��Bottom=�#��z5 6 . |�.  $��Slide=�.��� $��DropDown��+ . #��z7 #��z8 $
��Auto��.+��� $ DropDownBorder��8�� $��1,0,1,1q����#ADAAAD�� $��4,6,4,6q�+ . 9 ���� '��       " ��.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . : P�  �@��Orientation$ 
Horizontal=�. ��� 	 � ��$

 Auto���� 	 � ��$
 18���� 	 � ��8
  P�  �@��Orientation$ Vertical=�. ��� 	 � ��$
 18���� 	 � ��$

 Auto���� 	 � ��8
  .���� ; . $< 	Contained=� P�  �@��HasItems .  . ��� $ DropDownBorder��	 � ��$
 95��P�  � {� .  . ��� $ PART_EditableTextBox��8
  	 � ���� $ ToggleButton��	 � {�
 .  P�  � {� . . ��� $ ToggleButton��	 � {�
 . P�  �@��
IsGrouping . . ��� 	 � ��
 .  P� $= Popup�� �@�AllowsTransparency . . ��� $ DropDownBorder��	 � ��$
 0,0,0,0q�P�  �@��
IsEditable . . ��� 	 � ��
 .  �� $ PART_EditableTextBox��	 � y�$
 Visible=��� $ ContentSite��	 � y�$
 Hidden=�: �� " ��.��� 	 � ��
 �� .R��� $> Close��$B��</Agent.Interaction.Desktop;component/Images/Custom Close.png��$��10��$��10��? �� $
@ True��A �� $ �� $B FindAncestor=�"C ��$# IsMouseOver�.a��� $ Close��	 � ��$K
 E/Agent.Interaction.Desktop;component/Images/Custom Close selected.png���� $
@ True��A �� $ �� $B FindAncestor=�"C ��$# 	IsPressed�.a��� $ Close��	 � ��$K
 E/Agent.Interaction.Desktop;component/Images/Custom Close selected.png���� 	 � ��
 �� " ��D ���� OnMouseEnter14 .��U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #+ z+ #��z��z�V� .~��� $H 1Q�$& 1Q��� $I 0Q�$J 0Q��� $ 0Q�T� $K 0Q�$L 0Q� :� $M Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#d6ebffP�  � �� . . ��� $ Border��	 � ��
 �#94c3e7�� " ��.��� 	 � ��$
 1q��� 	 � ��

 ���� �� 	 � ��
 �� " ��.R��� $��border��#��z��._��� $��content��#��z��#��z����. #+ z+ #��z������ " ��: P�  �@J�IsMouseOver . . ��� 	 � ��
 �#858E98�� 	 � ��$
 1q��� 	 �@��CornerRadius$
 3,3,3,3�� P�  �@J�IsMouseOver . . ��� $ border��	 � ��
 �#d6ebffP�  � �� . . ��� $ border��	 � ��
 �#94c3e7�� $ border��	 � ��
 �#858E98�� $ border��	 � ��$
 1q��� $ border��	 �@��CornerRadius$
 3��P�  �@J�	IsFocused . �� " ��.��� 	 � ��$
 1q��� 	 � ��
 �#939589�� 	 � ��
 �� " ��.R��� $��border��#��z��$N 2��._��� $��content��#��z��#��z����. #+ z+ #��z������ " ��: P�  �@J�IsMouseOver . . ��� 	 � ��
 �#939589�� 	 � ��$
 1q��� 	 �@��CornerRadius$
 2�� P�  �@J�IsMouseOver . P�  � �� . . ��� 	 � ��
 �#939589�� 	 � ��$
 1q�P�  �@J�	IsFocused . �� " ��.��� 	 � ��
 �� .R��� $> Minimize��$E��?/Agent.Interaction.Desktop;component/Images/Custom Minimize.png��$��0,-1,0,0q�$��14��$��11��? �� $
@ True��A �� $ �� $B FindAncestor=�"C ��$# IsMouseOver�.a��� $ Minimize��	 � ��$N
 H/Agent.Interaction.Desktop;component/Images/Custom Minimize selected.png���� $
@ True��A �� $ �� $B FindAncestor=�"C ��$# 	IsPressed�.a��� $ Minimize��	 � ��$N
 H/Agent.Interaction.Desktop;component/Images/Custom Minimize selected.png���� 	 � ��
 �� " ��D ���� OnMouseEnter14 .��U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #+ z+ #��z��z�V� .~��� $H 1Q�$& 1Q��� $I 0Q�$J 0Q��� $ 0Q�T� $K 0Q�$L 0Q� :� $M Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#d6ebffP�  � �� . . ��� $ Border��	 � ��
 �#94c3e7�� " ��.��� 	 � ��
 �� " ��D ���� OnMouseEnter14 .��U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #+ z+ #��z��z�V� .~��� $H 1Q�$& 1Q��� $I 0Q�$J 0Q��� $ 0Q�T� $K 0Q�$L 0Q� :� $M Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#d6ebffP�  � �� . . ��� $ Border��	 � ��
 �#94c3e7�� " ��: �� A �� $ �� *Self+$# IsKeyboardFocusWithin�@ True.a��� 	 � ��

 ���� �� A �� $ �� *Self+$# IsMouseOver�@ True.a��� 	 � ��
 �#d6ebff�� " ��.��� 	 � ��
 �� " ��.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #+ z+ #��z�� P�  �@J�IsMouseOver . . ��� $ Border��	 � ��

 ���� P�  � �� . . ��� $ Border��	 � ��
 �#94c3e7�� " ��.��� 	 � ��
 �� " ��.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #+ z+ #��z�� P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#d6ebffP�  � �� . . ��� $ Border��	 � ��
 �#94c3e7�� " ��.��� 	 � ��
 �� " ��.R��� $��Border��
������ ���#D6D7D6._��� $��content��#��z��#��z����. #+ z+ #��z�� P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#D6D7D6P�  � �� . . ��� $ Border��	 � ��
 �#D6D7D6�� " ��.��� 	 � ��
 �� " ��.R��� $��Border��$N 2�����#4DBF4D
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #+ z+ #��z�� :� $M Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$O 1X�$
 0.25X�$ 0:0:0.25D�P . P�  � �� . . ��� $ Border��	 � ��
 �#358535�� " ��.��� 	 � ��
 �� " ��.R��� $��Border��$N 2�����#f94e4b
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #+ z+ #��z�� P�  � �� . . ��� $ Border��	 � ��
 �#EF0C08�� " ��.��� 	 � ��
 �� " ��.R��� $��Border��$N 2�����#FFCC00
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #+ z+ #��z�� :� $M Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$O 1X�$
 0.25X�$ 0:0:0.25D�P . P�  � �� . . ��� $ Border��	 � ��
 �#b28e00�� " ��.��� 	 � ��
 �� " ��.R��� $ Border��$N 0,0,2,2��
������ 
������ $��1,0,1,1q�._�F� $ Arrow��
�������$��Center=�$��Bottom=�"����        � 	=  	= � z     	 P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#007edfP�  � �� . . ��� $ Border��	 � ��
 �#0061ac�� " ��.��� 	 � ��
 �� " ��.R��� $��Border��$N 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #+ z+ #��z�� P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#007edf:� $M Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$O 1X�$
 0.25X�$ 0:0:0.25D�P . P�  � �� . . ��� $ Border��	 � ��
 �#0061ac�� " ��.��� 	 � ��
 �� " ��.R��� $��Border��$N 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #+ z+ #��z�� P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#007edf:� $M Button.Click��.[��� .���� .��W� $ (Button.Opacity)�$O 1X�$
 0.25X�$ 0:0:0.25D�P . P�  � �� . . ��� $ Border��	 � ��
 �#0061ac�� " d�.��� 	 � ��
 �� " d�.R��� $ Border��$N 2��$��1q����#9395898��  ._��� $��2q�$��Center=�$��Center=���.  P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#1398ff�� 	 �@�Cursor$

 Hand��P�  � �� . . ��� $ Border��	 � ��
 �#0061acP�  �@d�	IsChecked$
 trueY�. ��� $ Border��	 � ��
 �#007edf�� " ��.��� 	 � ��

 �   ��� 	 � ��$
 Calibri)��� 	 � ��$
 12(��� "  .��� 	 � ��
 �� "  .R��� #��zQ #��z��#��z��#��z��#��z��#��z��#��z��#��z��#��z��#��z���� .��� 	 � ��
 �� .R��� ._��� $��15,0,0,0q�$��1�����	#60000000$	R 2 2R��� " ��.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . �� 	 �@�FocusVisualStyle8
   �� 	 � ��
 �� " ��.R��� $��Center=�
S ���� T �� $ Border��$��13��$��13��$N 0��
�������$��1q����#ADAAAD._�F� $ 	CheckMark��$��7��$��7��+ .  8�� $��2��+����        ���j ��j �    ��j ���j     	._��� $��4,0,0,0q�$��Center=�$
��Left=���.  P�  �@d�	IsChecked$ falseY�. ��� $ 	CheckMark��	 � y�$
 	Collapsed=�P�  Z�  �@d�	IsChecked. ��� $ 	CheckMark��	 � ��
 ��    ��j ���j     	P�  �@J�IsMouseOver . . ��� $ Border��	 � ��

 �   �P�  � �� . . ��� $ Border��	 � ��
 �#F7F3F7�� $ Border��	 � ��

 �   �P�  � {� .  . ��� $ Border��8
  	 � ���� $ Border��	 � ��
 �	#40000000�� 	 � ��
 �	#80000000�� " ��.��� 	 � ��
 �� " ��.R�� ' �� $
��Auto ��� $��* �.+��� $��0��$U 1��$��0,0,0,0q�; . 
��������� $��1�����#D6D7D6$��1q�._��� $��SelectedContent���� " ��.��� 	 � ��
 �� " ��.R�� .+��� $��Border��
�������$��1,1,1,0q�
������ $��Calibri)�$��12(�._��� $ ContentSite��$��Center=�$��Center=�$��Header��$��20����. $��5,5,5,0q� P�  �@��
IsSelected . . ��� $ Border��	 � ��
 �#0071C6�� $ Border��	 � ��
 �#D6D7D6�� $ Border��	 � ��$
 0,0,0,-2q��� $ ContentSite��	 � ��$
 5,2,5,0q��� " ��.��� 	 � ��
 �� " ��.R��� $ 0,3,0,3q����#ADAAAD$
��1.25q�
�������+ . ._��� �� " ��.��� 	 � ��
 �� " ��.R��� $ 0,3,0,3q����#ADAAAD$
��1.25q�
�������+ . ._��� �� " m�.R��� $��Border��._�� .+��� $��	-10,3,6,3q�$��Header����. � $��Popup��$��Bottom=�#��zV 6 . |�.  $
��Fade=�.���� $��SubmenuBorder��+ . ���#FFFFFF���#888888$��1q�._��� ; . $< Cycle=� P� " �@m�IsSuspendingPopupAnimation . . ��� $ Popup��	 � ��$

 None=�P�  �@m�IsHighlighted . . ��� $ Border��8
   	 � ���� $ Border��	 � ��

 ���� P� $= Popup�� �@�AllowsTransparency . . ��� $ SubmenuBorder��	 �@��Padding$
 0,3,0,3q��� $ SubmenuBorder��8
  	 � ���� $ SubmenuBorder��	 � ��$

 1.25q�P�  � {� .  . ��� 8
  	 � ���� " m�.R��� $��Border��._�� .+��� $��6,3,6,3q�$��Header����.  P�  �@m�IsHighlighted . . ��� $ Border��8
   	 � ���� $ Border��	 � ��

 ���� P�  � {� .  . ��� 8
  	 � ���� " m�.R��� $��Border��$ 0,3,0,3q�._��  �� $
��Auto �$
W Icon���� $��* ��� $
��Auto �$W Shortcut���� $��13 �.+��� $
��Icon��$��6,0,6,0q�$��Center=�$
��Icon���� $��Check��$��13��$��13��$y�	Collapsed=�8��  $��1q�8�� ._�F� $��	CheckMark��$��7��$��7��$y�Hidden=�+ .  8�� $��2��+����        ���j ��j �    ��j ���j     	�� $��
HeaderHost��$��1��$��Header����. �� $X InputGestureText��$��2��#��zY $��2,0,2,0q�$��Right=� P�  Z�  �@m�Icon. ��� $
 Icon��	 � y�$
 Hidden=�P�  �@m�	IsChecked . . ��� $ 	CheckMark��	 � y�$
 Visible=�P�  �@m�IsCheckable . . ��� $ Check��	 � y�$
 Visible=��� $
 Icon��	 � y�$
 Hidden=�P�  �@m�IsHighlighted . . ��� $ Border��8
  	 � ��P�  � {� .  . ��� 8
  	 � ���� " m�.R��� $��Border��$ 0,3,0,3q�._��  �� $
��Auto �$
W Icon���� $��* ��� $
��Auto �$W Shortcut���� $��10 �.+��� $
��Icon��$��Center=�$
��Icon���� $��
HeaderHost��$��1��$��Header����. �� $X InputGestureText��$��2��#��zY $��2,0,2,0q�$��Right=�F� $��3��$��Center=�$��Center=�"����        �    ��j � 	= �g5 	8��  � $��Popup��$��Right=�#��zV 6 . |�.  $��Slide=�.���� $��SubmenuBorder��+ . 8�� ._��� ; . $< Cycle=� P�  Z�  �@m�Icon. ��� $
 Icon��	 � y�$
 Hidden=�P�  �@m�IsHighlighted . . ��� $ Border��8
  	 � ��P� $= Popup�� �@�AllowsTransparency . . ��� $ SubmenuBorder��	 �@��Padding$
 0,3,0,3q��� $ SubmenuBorder��8
  	 � ���� $ SubmenuBorder��	 � ��$

 1.25q�P�  � {� .  . ��� 8
  	 � ���� " m�.��� 	 �@�OverridesDefaultStyle
 . : P�  �@m�Role$ SubmenuHeader=�. ��� 	 � ��8
   P�  �@m�Role$ SubmenuItem=�. ��� 	 � ��8
  P�  �@m�Role$ TopLevelHeader=�. ��� 	 � ��8
  �� 	 �@�IsSharedSizeScope
 . P�  �@m�Role$ TopLevelItem=�. ��� 	 � ��8
  �� "  .��� 	 � ��

 ������� 	 � ��
 �� "  .R��� #��z��#��z��#��z��+ . # z��._��� $��DG_ScrollViewer��|�.  ���� " ��.R�� ' �� $
��Auto ��� $��* ��� $
��Auto � �� $
��Auto ��� $��* ��� $
��Auto �.+��� #��Z^ |�.  ���� $ �� "C  $ # CellsPanelHorizontalOffset���C� *�� "_  $"` DataGridSelectAllButtonStyle��+y��� $ �� "C  $# HeadersVisibility�#b Za #d Zc   $!��PART_ColumnHeadersPresenter��$��1��y��� $ �� "C  $# HeadersVisibility�#b Za #d Ze �� $!f PART_ScrollContentPresenter��$��1��$��2��#g z���� $��PART_VerticalScrollBar��$��1��$��1��$h Vertical=�$��Right=�$��0,0,0,0q�+ . ! . #j zi #l zk #y�zm n �� $# VerticalOffset�$ �� *TemplatedParent+$- OneWay=����� " ��.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . : P�  �@��Orientation$ Vertical=�. ��� 	 � ��$
 18���� 	 � ��$

 Auto���� 	 � ��8
   � $��2��$��1�� �� ���� $ �� "C  $.# (NonFrozenColumnsViewportHorizontalOffset��� $��* �.+��� $��PART_HorizontalScrollBar��$��1��$h 
Horizontal=�#j zo #l zp #y�zq n �� $# HorizontalOffset�$ �� *TemplatedParent+$- OneWay=����� " ��.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . : P�  �@��Orientation$ 
Horizontal=�. ��� 	 � ��$

 Auto���� 	 � ��$
 18���� 	 � ��8
  .���� #+ z+ �� "  : P�  �@ 
IsSelected . . ��� 	 � ��
 Z� �� 	 � ��
 Z� �� 	 � ��

 �   ��� " n�.��� 	 � ��$
 5���� 	 � ��

 ���� �� 	 �@�Cursor$
 SizeWE���� 	 � ��
 �� " n�.R��� #��z��# z���� "  .��� 	 � ��
 �� "  .R�� .+��� �� "  .��� 	 � ��$
 Center=��� 	 � ��$
 Stretch=��� 	 � ��$
 Center=��� 	 � ��

 �   ��� 	 � ��$
 12(��� 	 � ��$

 Bold"��� 	 � ��$
 Calibri)��� 	 � ��
 �� "  D ���� HoverOn4 .��U� $E 00:00:00X�$ 
MainBorder��$ (UIElement.Opacity)�.\��� $F 00:00:00.2000000��$	G 0.6Q��� HoverOff4 .��U� $E 00:00:00X�$ 
MainBorder��$ (UIElement.Opacity)�.\��� $F 00:00:00.3000000��$G 1Q�.R�� .+��� $��
MainBorder��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
�������._�� .+��� $��3q�$��Center=�$��Center=�#��z��#s zr #��z��F� $��UpArrow��
���   �$��Right=�$��Center=�$��0,2,1,0q�$y�Hidden=���B� .��E� $	t 0,0&�.���� $	u 7,0&��� $u 3.5,6&��� $	u 0,0&�F� $��	DownArrow��
���   �$��Right=�$��Center=�$��0,2,1,0q�$y�Hidden=���B� .��E� $	t 0,6&�.���� $	u 7,6&��� $u 3.5,0&��� $	u 0,6&�n� $v PART_LeftHeaderGripper��$��0��$
��Left=�8��  n� $v PART_RightHeaderGripper��$��3��$��Right=�8��  P�  �@ SortDirection$ 
DescendingX�. ��� $ UpArrow��	 � y�$
 Hidden=��� $ 	DownArrow��	 � y�$
 Visible=�P�  �@ SortDirection$ 	AscendingX�. ��� $ 	DownArrow��	 � y�$
 Hidden=��� $ UpArrow��	 � y�$
 Visible=�P�  �@J�IsMouseOver . . ��� 	 �@�Cursor$

 Hand���� $ 
MainBorder��	 � ��

 ������� $ 
MainBorder��	 � ��

 �   ��� 	 � ��
 �#0071C6�� "  .��� 	 � ��$
 Center=��� 	 � ��

 ������� 	 � ��
 �� "  .R�� .+��� $��
MainBorder��$	��0.5q�
���   �8��  ._�� .+��� $��3q�$��Center=�$��Center=�#��z��#s zr #��z���� " ��.��� 	 � ��
 �� .R��� $> Minimize��$G��A/Agent.Interaction.Desktop;component/Images/Maximize1.Disable.png��$��15��$��11��? �� $
@ True��A �� $ �� $B FindAncestor=�"C ��$# IsMouseOver�.a��� $ Minimize��	 � ��$?
 9/Agent.Interaction.Desktop;component/Images/Maximize1.png���� $
@ True��A �� $ �� $B FindAncestor=�"C ��$# 	IsPressed�.a��� $ Minimize��	 � ��$?
 9/Agent.Interaction.Desktop;component/Images/Maximize1.png���� 	 � ��
 �� " ��D ���� OnMouseEnter14 .��U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #+ z+ #��z��z�V� .~��� $H 1Q�$& 1Q��� $I 0Q�$J 0Q��� $ 0Q�T� $K 0Q�$L 0Q� :� $M Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#d6ebffP�  � �� . . ��� $ Border��	 � ��
 �#94c3e7�� " ��.��� 	 � ��
 �� .R��� $> Minimize��$D��>/Agent.Interaction.Desktop;component/Images/Normal.Disable.png��$��15��$��11��? �� $
@ True��A �� $ �� $B FindAncestor=�"C ��$# IsMouseOver�.a��� $ Minimize��	 � ��$<
 6/Agent.Interaction.Desktop;component/Images/Normal.png���� $
@ True��A �� $ �� $B FindAncestor=�"C ��$# 	IsPressed�.a��� $ Minimize��	 � ��$D
 >/Agent.Interaction.Desktop;component/Images/Normal.Disable.png���� 	 � ��
 �� " ��D ���� OnMouseEnter14 .��U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�U� $E 00:00:00X�$ content��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $F 00:00:00��$	G 1.4Q��� $F 00:00:01��$G 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #+ z+ #��z��z�V� .~��� $H 1Q�$& 1Q��� $I 0Q�$J 0Q��� $ 0Q�T� $K 0Q�$L 0Q� :� $M Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border��	 � ��
 �#d6ebffP�  � �� . . ��� $ Border��	 � ��
 �#94c3e7�� " ��: P�  �@��AlternationIndex$ 0��. ��� 	 � ��

 ��·�P�  �@��AlternationIndex$ 1��. ��� 	 � ��

 �����P�  �@��
IsSelected . . ��� 	 � ��

 � ����� .R�]� 
�������.+��� $��10(�
���   �$
��Bold"����� �� " ��.��� 	 �@�OverridesDefaultStyle
 . �� 	 � ��
 .  �� 	 � |�
 .  �� 	 � ��
 �� " ��.R��� 
������ �� " n�.��� 	 �@�OverridesDefaultStyle
 . �� 	 � ��
 �� " n�.R�A� ���#DEDEDE���#99BBE8$��15��$��15���� " ��.��� 	 � ��
 �� " ��.R��� #��z��#��z��#��z��._�� ' �� $
��Auto ��� $
��Auto �#��z���� $
��Auto �.+�m� $w TopTick��$y�	Collapsed=�#x z��$	y Top=�$��4��$��0��m� $w 
BottomTick��$y�	Collapsed=�#x z��$y Bottom=�$��4��$��0���� $ TrackBackground��8��  8�� $��1q�$N 1��$	��5,0q�$��Center=�$	��4.0��$��1��._��� $��-6,-1q�.+��� $z PART_SelectionRange��$y�Hidden=�$	��4.0��#��� ��#��� ��$	��1.0��]� ${ 
PART_Track��$��1��( �� 8�� #��Z| * �� 8�� #��Z} ) n� $v Thumb��8�� 
���   ��� " ��.��� 	 � ��$
 Center=��� 	 � ��$
 Stretch=��� 	 � ��$
 Center=��� 	 � ��

 �   ��� 	 � ��$
 12(��� 	 � ��$

 Bold"��� 	 � ��$
 Calibri)��� 	 � ��
 �� " ��D ���� HoverOn4 .��U� $E 00:00:00X�$ 
MainBorder��$ (UIElement.Opacity)�.\��� $F 00:00:00.2000000��$	G 0.6Q��� HoverOff4 .��U� $E 00:00:00X�$ 
MainBorder��$ (UIElement.Opacity)�.\��� $F 00:00:00.3000000��$G 1Q�.R�� .+��� $��
MainBorder��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
�������._�� .+��� $��3q�$��Center=�$��Center=�#��z��#s zr #��z��n� $v PART_LeftHeaderGripper��$��0��$
��Left=�8��  n� $v PART_RightHeaderGripper��$��3��$��Right=�8��  P�  �@J�IsMouseOver . . ��� 	 �@�Cursor$

 Hand���� $ 
MainBorder��	 � ��

 ������� $ 
MainBorder��	 � ��

 �   ��� 	 � ��
 �#0071C6