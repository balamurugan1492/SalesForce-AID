   M S B A M L   `   `   `  ���� T  PPointel.Interactions.Chat, Version=5.0.3.5, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Pointel.Interactions.Chat.CustomControls(Pointel.Interactions.Chat.CustomControls  p lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   DisabledBorderBrush  �� 
StartPoint �� EndPoint � Color � Offset  TabItemBrush  	GridBrush  GradiantBrush  GradiantBrush1  
OpticBrush  GradiantBrush2  	DarkBrush  PressedBrush 	 NormalBrush 
 NormalBrush1  HorizontalNormalBrush  
LightBrush  HorizontalLightBrush  NormalBorderBrush  HorizontalNormalBorderBrush  DefaultedBorderBrush  PressedBorderBrush  Tile  
GlassBrush  CBorderBrush % !HorizontalSliderTrackNormalBorder  	WavyBrush j� Viewbox j� ViewboxUnits j� Viewport j� ViewportUnits j� TileMode	 !� Visual
 J� SnapsToDevicePixels ) %HorizontalSliderTrackNormalBackground �� Color 
 CallBG  ClearBG  NormalBG  DisabledForegroundBrush  DisabledBackgroundBrush  WindowBackgroundBrush  SelectedBackgroundBrush  LightBorderBrush   
GlyphBrush ! LightColorBrush " SolidBorderBrush # DisabledBorderBrush1 $ HighlightedBrush % controltempTextbox �� 
TargetType �� Padding ��Name & controltempTextbox1 ' MyWaterMarkStyle �� 
TargetType �� Property �� Value ��Name
 � Tag �� Triggers ]� 
Conditions �� Property �� Value �� 
TargetName P� Property P� Value ( controltempPasswordbox ) AnimatedExpanderButtonTemp ��Name � ColumnDefinitions F�Name J� RenderTransformOrigin �� Angle  ��Name! N� EnterActions" �� 
TargetName# �� TargetProperty	$ W� To% g� Duration& N� ExitActions * StretchyExpanderTemp' d�Name( � OverridesDefaultStyle) d� 	IsChecked* �� Path+ �� RelativeSource, � LayoutTransform- �� ScaleY + SimpleExpanderButtonTemp , SimpleExpanderTemp - ComboBoxToggleButton . ComboBoxTextBox / ScrollBarLineButton 0 ScrollBarPageButton 1 ScrollBarThumb 2 VerticalScrollBar. � RowDefinitions/ ]� DecreaseRepeatButton0 ]� Thumb1 ]� IncreaseRepeatButton 3 HorizontalScrollBar2 �� 	ClickMode3 �� Mode4 J� IsHitTestVisible5 �� SelectionBoxItem6 �� SelectionBoxItemTemplate7 ��Name8 �� 	UndoLimit9 �� 
IsReadOnly: �� 
IsReadOnly; �� IsDropDownOpen< � AllowsTransparency= � ActualWidth> �� MaxDropDownHeight? � 	Resources@ �� TriggersA K� IsItemsHost-   'System.Windows.Input.KeyboardNavigationB    DirectionalNavigationC P� 
SourceName 4 CloseButtonD ��NameE �� TriggersF �� ValueG �� BindingH �� ModeI �� AncestorTypeJ � 	Resources 5 KeyK g� 	BeginTimeL O� KeyTimeM O� ValueN �� ScaleXO �� AngleXP �� AngleYQ T� XR T� YS :� RoutedEvent 6 OnMouseEnter1 7 numberButton 8 numberButton1T �� CornerRadius 9 MinimizeButton : 	PinButton ; TCStackPannelImageButton < TCArrowImageButton = NormalImageButton > NormalImageButton1 ? 
CallButtonU W� FromV g� AutoReverse @ RejectButton A ClearButton B NormalButton1 C NormalButton D ChatSendButton E 
SideButton9   3Pointel.Interactions.Chat.CustomControls.TimerLabelW   TextX K� ZIndex F Contextmenu G DefaultCopyContextmenuY m� CommandZ m� IsSubmenuOpen[ z� SharedSizeGroup\ ��Name] m� InputGestureText H Menuitem^ m� SubmenuHeaderTemplateKey_ m� SubmenuItemTemplateKey ` m� TopLevelHeaderTemplateKeya m� TopLevelItemTemplateKey)  #Microsoft.Windows.Controls.DataGridb   SelectAllCommandc �� TypeInTargetAssemblyd �� 
ResourceId!e   HeadersVisibilityConverterf �� 	Converter:  4Microsoft.Windows.Controls.DataGridHeadersVisibility
g   Allh �� ConverterParameterJ  DMicrosoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenteri   Columnj ��Namek �� CanContentScrolll �� Orientationm �� ScrollableHeightn � Maximumo �� ViewportHeightp �� ViewportSize*q �� #ComputedVerticalScrollBarVisibilityr � Values �� ScrollableWidtht �� ViewportWidth,u �� %ComputedHorizontalScrollBarVisibility-  'Microsoft.Windows.Controls.DataGridCell I ColumnHeaderGripperStyle@  :Microsoft.Windows.Controls.Primitives.DataGridColumnHeaderv �� ContentStringFormatw �� ContentStringFormatx E� 
StartPointy �� Pointz n�Name=  7Microsoft.Windows.Controls.Primitives.DataGridRowHeader J maximizeButton K RestoreButton L alternatingStyle M OrangeHeaderTemplate N SliderRepeatButtonStyle O CustomThumbForSlider P MyCustomStyleForSlider{ m�Name| m� Fill} m� 	Placement~ ��Name ]�Name� �� DecreaseLarge� �� IncreaseLarge Q CustomHeaderStyle�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     6my-http://schemas.microsoft.com/wpf/2008/toolkit  @local6clr-namespace:Pointel.Interactions.Chat.CustomControls  ]autoTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit  :x,http://schemas.microsoft.com/winfx/2006/xaml     % }  &	        &	 �     &	 A    &	 �    &	      &	 n    &	 �    &	     &	 p    &		 �    &	
 P    &	 �    &	 �    &	 S    &	 �    &	 �    &	 U    &	 �    &	     &	 w    &	 �    &	 D    &	 �    &	 >	    &	 Q	    &	 d	    &	 w	    &	 �	    &	 �	    &	 �	    &	 �	    &	 �	    &	  �	    &	! �	    &	" �	    &	# 

    &	$ 
    &	% -
    &	& �
    &	'     &	( �    &	)     &	*     7 ) &	+ �    &	, U    7 + &	- �    &	. �    &	/ &    &	0     &	1 �    &	2 �    7 / 7 0 7 1 7  7  7 0 7 / &	3 5    7 / 7 0 7 1 7 	 7  7 0 7 / '�� �    7 $ '�� D!    7 - 7  7 3 7 2 7  &	4 (    7 6 &	7 -    &	8 ~/    &	9 F1    7 6 &	: q6    7 6 &	; �9    &	< Z:    &	= g;    &	> x<    &	? �=    &	@ �>    &	A �?    &	B QA    &	C �B    &	D [D    &	E 
F    7  '�� �G    '  �G    '�� eH    '�� 0I    '�� �J    &	F OK    &	G �K    7 F (�� aL    *$"MenuItem.TopLevelHeaderTemplateKey+)7 $ 7  7  (�� O    *" MenuItem.TopLevelItemTemplateKey+)7 $ 7  (�� �O    *!MenuItem.SubmenuItemTemplateKey+)7 $ 7  (�� ?S    *#!MenuItem.SubmenuHeaderTemplateKey+)7   7  7 $ 7  7  &	H oV    7^ 7_ 7` 7a '  �W    7 2 7 3 '  �]    &	I .^    '  �^    '  �^    7 I 7 I '  �d    7 	 &	J �e    7 6 &	K �j    7 6 &	L �o    &	M zp    &	N �p    &	O Oq    &	P �q    7  7  7 N 7 N 7 O &	Q "t    7 I 7 I �� $	  0,0&�$	 0,1&���� � $ Orange��$ 0Q�� $ #C0C0C1��$
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
 0.55Q�� $ #808080��$ 1Q��� $	  0,0&�$	 1,0&�.��� $ 0Q�$ 	#66ffffff��� $ 0Q�$ 	#36FFFFFF��� $ 1Q�$ 	#B5EBE7E7���� $	  0,0&�$	 0,1&���� � $ #707f8b��$ 1Q�� $ #c6cdd1��$	 0.1Q��� $	 0,1&�$	  0,0&�.��� $ 	#FFAEB1AF��$	 0.1Q�� $ White��$ .9Q�!� $ 0,0,3,3�$ Absolute=�$ 	0,-1,7,15�$ Absolute=�$
 Tile=�	 F� -��������> �@B     �� ��  �0  5 	���	#FFFF0000$	��0.2��
 . �� $ #DEDEDE���� $ #358535���� $ #b28e00���� $ #0061ac���� $
 #888���� $
 #EEE���� $
 #FFF���� $
 #DDD���� $
 #AAA���� $
 #444���� $
 #DDD���� $
 #888���� $
 #AAA���� $ #CEE7F7���� " ��.R��� $ 2q�
����������#ADAAAD$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " ��.R��� $ 2q�
�������
������ $��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " ��.���  � �� �� " ��.R�� .+��� 
�������
������ $��1q��� $ PART_ContentHost��$��5,0,0,0q�$��Center=��� $ WaterMarkLabel��$��5,0,0,0q�#��z $��Center=�$y�	Collapsed=�
�������$��Calibri)�$��12(�$��Italic$� ]�  ��  � ��$  ��./��� $ WaterMarkLabel�� � y�$ Visible=�P�  � {� .  . ���  � ��
 �iii��� " G�.R��� $ 2q�
����������#ADAAAD$��1q�$��Calibri)�._��� $ PART_ContentHost��$��0q��� " d�.R��� $ ExpanderButtonBorder��#��z��#��z��#��z��# z��._�� $	��Top=� �� $
��Auto ��� $��* �.+��� 
������ $��2��A� $��Circle��$��0��
�������$��20��$��20��$��Center=�$��Center=�F� $ Arrow��$��0��"����@B `� � �D @KL � z `� 	���	#FF666666$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $ 0Q��� $  HeaderContent��$��1��$��4,0,0,0q�$��Center=�$��Content�� P�  �@d�	IsChecked$
 TrueY�! �� .���� .��W� $" Arrow��$4# .(Path.RenderTransform).(RotateTransform.Angle)�$	$ 180X�$% 0:0:0.4D�& �� .���� .��W� $" Arrow��$4# .(Path.RenderTransform).(RotateTransform.Angle)�$$ 0X�$% 0:0:0.4D�P�  �@J�IsMouseOver . . ��� $ Circle�� � �� �#808080P�  � �� . . ��� $ Circle�� � �� �#808080�� $ Arrow�� � ��
 ������� " 9�.R�]� .+�d� $' ExpanderButton��$��35��$	��Top=����#808080$��0,0,0,1q�8��  #��z��( . $��1.5,0q�) �� $* 
IsExpanded�+ �� *TemplatedParent+�� $  ExpanderContent��$��Content��$��Bottom=�, �� $- 0Q� P�  �@9�
IsExpanded . ! �� .���� .��W� $" ExpanderContent��$@# :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�$$ 1X�$% 0:0:0.4D�& �� .���� .��W� $" ExpanderContent��$@# :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�$$ 0X�$% 0:0:0.4D��� " d�.R��� $ ExpanderButtonBorder��#��z��#��z��#��z��# z��._��  �� $
��Auto ��� $��* �.+��� 
������ $��2��A� $��Circle��$��0��
�������$��20��$��20��$��Center=�$��Center=�F� $ Arrow��$��0��"����@B `� � �D @KL � z `� 	
�������$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $ 0Q��� $  HeaderContent��$��1��$��4,0,0,0q�$��Content�� P�  �@d�	IsChecked$
 TrueY�! �� .���� .��W� $" Arrow��$4# .(Path.RenderTransform).(RotateTransform.Angle)�$	$ 180X�$% 0:0:0.4D�& �� .���� .��W� $" Arrow��$4# .(Path.RenderTransform).(RotateTransform.Angle)�$$ 0X�$% 0:0:0.4D��� " 9�.R��� ���#FEC732$��1q�._�]� .+�d� $' ExpanderButton��$	��Top=�8��  #��z��( . $��1.5,0q�) �� $* 
IsExpanded�+ �� *TemplatedParent+�� $  ExpanderContent��$��1��$y�	Collapsed=�$��Bottom=� P�  �@9�
IsExpanded . . ��� $ ExpanderContent�� � y�$ Visible=��� " d�.R��  �� �� $��20 �.+��� $ Border��$��2��
����������#ADAAAD$��1q��� $��0��$��1q�
������ $��0,0,0,0q�F� $ Arrow��$��1�����#737573$��Center=�$��Center=�"����        � 	=  	= � z     	 P�  �@J�IsMouseOver . P�  �@d�	IsChecked$
 trueY�P�  � {� .  �� " ��.R��� $ PART_ContentHost��|�.  #��z���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � |� .  ��  � �� �� " ��.R��� $��Border��$��1q�
����������#ADAAAD$��1q�._�F� $��Center=�$��Center=����#737573���� $* Content�+ �� *TemplatedParent+ P�  �@J�IsMouseOver . . ��� $ Border�� � ��
 �   �P�  � �� . . ��� $ Border�� � ��
 �   ��� $ Border�� � �� �#F7F3F7P�  � {� .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " ��.R��� ���#F7F3F7�� " n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " n�.R��� $��Border��
����������#ADAAAD$��1q� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#F7F3F7P�  �@n�
IsDragging . . ��� $ Border�� � �� �#F7F3F7�� $ Border�� � ��
 �   �P�  � {� .  �� " ��.R�� . �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����. / �� 8�� $��ScrollBar.PageUpCommand��0 n� 8�� $��1,0,1,0q�8�� 8�� 1 �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.  / �� 8�� $��ScrollBar.PageLeftCommand��0 n� 8�� $��0,1,0,1q�8�� 8�� 1 �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� " ��.���  � �� �#737573��  � ��$ 20����  � ��$ Center=���  � �� �� " ��.R��� $ Bd��
 . #��z��#��z��#��z��# z��._��� #��z��#��z��#
 z
  P�  �@��IsHighlighted . . ��� 8    � ����  � ��$ 12(�P�  � {� . . ���  � ��$ 12(�P�  � {� .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  � �� �� " ��.R�� .+�d� $��ToggleButton��8��  $��2��|�.  $2 Press=�) �� $* IsDropDownOpen�$3 TwoWay=�+ �� *TemplatedParent+�� $��ContentSite��4 .  #��z5 #��z6 #��z��$��3,3,23,3q�$��Center=�$
��Left=��� $7 PART_EditableTextBox��$8 10��$��LeftToRight=�$	��210��$
��Left=�
������ $	��Top=�$��25��$
��Left=�$	��Top=�$��3,3,23,3q�|�. 
������ $y�Hidden=�#: z9 ��Z� � $��Popup��$��Bottom=�#��z; < . |�.  $��Slide=�.��� $��DropDown��
 . #��z= #��z> $
��Auto��.+��� $ DropDownBorder��8�� $��1,0,1,1q����#ADAAAD�� $��4,6,4,6q�
 . ? ���� '��       " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . @ P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  .���� A . $B 	Contained=� P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$ 95��P�  � {� .  . ��� $ PART_EditableTextBox��8   � ���� $ ToggleButton�� � {� .  P�  � {� . . ��� $ ToggleButton�� � {� . P�  �@��
IsGrouping . . ���  � �� .  P� $C Popup�� �@�AllowsTransparency . . ��� $ DropDownBorder�� � ��$ 0,0,0,0q�P�  �@��
IsEditable . . ���  � �� .  �� $ PART_EditableTextBox�� � y�$ Visible=��� $ ContentSite�� � y�$ Hidden=�@ �� " ��.���  � �� �� .R��� $D Close��$B��</Agent.Interaction.Desktop;component/Images/Custom Close.png��$��10��$��10��E �� $
F True��G �� + �� $H FindAncestor=�"I ��$* IsMouseOver�.a��� $ Close�� � ��$K E/Agent.Interaction.Desktop;component/Images/Custom Close selected.png���� $
F True��G �� + �� $H FindAncestor=�"I ��$* 	IsPressed�.a��� $ Close�� � ��$K E/Agent.Interaction.Desktop;component/Images/Custom Close selected.png����  � �� �� " ��J ���� OnMouseEnter15 .��U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #
 z
 #��z��z�V� .~��� $N 1Q�$- 1Q��� $O 0Q�$P 0Q��� $ 0Q�T� $Q 0Q�$R 0Q� :� $S Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��.���  � ��$ 1q���  � ��
 ���� ��  � �� �� " ��.R��� $��border��#��z��._��� $��content��#��z��#��z����. #
 z
 #��z������ " ��@ P�  �@J�IsMouseOver . . ���  � �� �#858E98��  � ��$ 1q���  �@��CornerRadius$ 3,3,3,3�� P�  �@J�IsMouseOver . . ��� $ border�� � �� �#d6ebffP�  � �� . . ��� $ border�� � �� �#94c3e7�� $ border�� � �� �#858E98�� $ border�� � ��$ 1q��� $ border�� �@��CornerRadius$ 3��P�  �@J�	IsFocused . �� " ��.���  � ��$ 1q���  � �� �#939589��  � �� �� " ��.R��� $��border��#��z��$T 2��._��� $��content��#��z��#��z����. #
 z
 #��z������ " ��@ P�  �@J�IsMouseOver . . ���  � �� �#939589��  � ��$ 1q���  �@��CornerRadius$ 2�� P�  �@J�IsMouseOver . P�  � �� . . ���  � �� �#939589��  � ��$ 1q�P�  �@J�	IsFocused . �� " ��.���  � �� �� .R��� $D Minimize��$E��?/Agent.Interaction.Desktop;component/Images/Custom Minimize.png��$��0,-1,0,0q�$��14��$��11��E �� $
F True��G �� + �� $H FindAncestor=�"I ��$* IsMouseOver�.a��� $ Minimize�� � ��$N H/Agent.Interaction.Desktop;component/Images/Custom Minimize selected.png���� $
F True��G �� + �� $H FindAncestor=�"I ��$* 	IsPressed�.a��� $ Minimize�� � ��$N H/Agent.Interaction.Desktop;component/Images/Custom Minimize selected.png����  � �� �� " ��J ���� OnMouseEnter15 .��U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #
 z
 #��z��z�V� .~��� $N 1Q�$- 1Q��� $O 0Q�$P 0Q��� $ 0Q�T� $Q 0Q�$R 0Q� :� $S Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��.���  � �� �� " ��J ���� OnMouseEnter15 .��U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #
 z
 #��z��z�V� .~��� $N 1Q�$- 1Q��� $O 0Q�$P 0Q��� $ 0Q�T� $Q 0Q�$R 0Q� :� $S Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��@ �� G �� + �� *Self+$* IsKeyboardFocusWithin�F True.a���  � ��
 ���� �� G �� + �� *Self+$* IsMouseOver�F True.a���  � �� �#d6ebff�� " ��.���  � �� �� " ��.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #
 z
 #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � ��
 ���� P�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��.���  � �� �� " ��.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #
 z
 #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��.���  � �� �� " ��.R��� $��Border��
������ ���#D6D7D6._��� $��content��#��z��#��z����. #
 z
 #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#D6D7D6P�  � �� . . ��� $ Border�� � �� �#D6D7D6�� " ��.���  � �� �� " ��.R��� $��Border��$T 2�����#4DBF4D
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #
 z
 #��z�� :� $S Button.Click��.[��� .���� .��W� $# (Button.Opacity)�$U 1X�$
$ 0.25X�$% 0:0:0.25D�V . P�  � �� . . ��� $ Border�� � �� �#358535�� " ��.���  � �� �� " ��.R��� $��Border��$T 2�����#f94e4b
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #
 z
 #��z�� P�  � �� . . ��� $ Border�� � �� �#EF0C08�� " ��.���  � �� �� " ��.R��� $��Border��$T 2�����#FFCC00
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #
 z
 #��z�� :� $S Button.Click��.[��� .���� .��W� $# (Button.Opacity)�$U 1X�$
$ 0.25X�$% 0:0:0.25D�V . P�  � �� . . ��� $ Border�� � �� �#b28e00�� " ��.���  � �� �� " ��.R��� $ Border��$T 0,0,2,2��
������ 
������ $��1,0,1,1q�._�F� $ Arrow��
�������$��Center=�$��Bottom=�"����        � 	=  	= � z     	 P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#007edfP�  � �� . . ��� $ Border�� � �� �#0061ac�� " ��.���  � �� �� " ��.R��� $��Border��$T 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #
 z
 #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#007edf:� $S Button.Click��.[��� .���� .��W� $# (Button.Opacity)�$U 1X�$
$ 0.25X�$% 0:0:0.25D�V . P�  � �� . . ��� $ Border�� � �� �#0061ac�� " ��.���  � �� �� " ��.R��� $��Border��$T 2�����#939589
�������$��Calibri)�$��14(�._��� $��content��#��z��#��z����. #
 z
 #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#007edf:� $S Button.Click��.[��� .���� .��W� $# (Button.Opacity)�$U 1X�$
$ 0.25X�$% 0:0:0.25D�V . P�  � �� . . ��� $ Border�� � �� �#0061ac�� " d�.���  � �� �� " d�.R��� $ Border��$T 2��$��1q����#9395898��  ._��� $��2q�$��Center=�$��Center=���.  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#1398ff��  �@�Cursor$
 Hand��P�  � �� . . ��� $ Border�� � �� �#0061acP�  �@d�	IsChecked$
 trueY�. ��� $ Border�� � �� �#007edf�� " ��.���  � ��
 �   ���  � ��$ Calibri)���  � ��$ 12(��� "  .���  � �� �� "  .R��� #��zW #��z��#��z��#��z��#��z��#��z��#��z��#��z��#��z��#��z���� " ��.���  � �� �� " ��.R�� . �� $
��Auto ��� $��* �.+��� $��0��$X 1��$��0,0,0,0q�A . 
��������� $��1�����#D6D7D6$��1q�._��� $��SelectedContent���� " ��.���  � �� �� " ��.R�� .+��� $��Border��
�������$��1,1,1,0q�
������ $��Calibri)�$��12(�._��� $  ContentSite��$��Center=�$��Center=�$��Header��$��20����. $��5,5,5,0q� P�  �@��
IsSelected . . ��� $ Border�� � �� �#0071C6�� $ Border�� � �� �#D6D7D6�� $ Border�� � ��$ 0,0,0,-2q��� $ ContentSite�� � ��$ 5,2,5,0q��� " ��.���  � �� �� " ��.R��� $ 0,3,0,3q����#ADAAAD$
��1.25q�
�������
 . ._��� �� " ��.���  � �� �� " ��.R��� $ 0,3,0,3q����#ADAAAD$
��1.25q�
�������
 . ._��� �� 8��  .@�m� $��Cut  ��$Y ApplicationCommands.Copy��m� $��Copy  ��$Y ApplicationCommands.Cut��m� $��Paste  ��$Y ApplicationCommands.Paste���� " m�.R��� $��Border��._�� .+��� $��	-10,3,6,3q�$��Header����. � $��Popup��$��Bottom=�#��zZ < . |�.  $
��Fade=�.���� $��SubmenuBorder��
 . ���#FFFFFF���#888888$��1q�._��� A . $B Cycle=� P� " �@m�IsSuspendingPopupAnimation . . ��� $ Popup�� � ��$
 None=�P�  �@m�IsHighlighted . . ��� $ Border��8    � ���� $ Border�� � ��
 ���� P� $C Popup�� �@�AllowsTransparency . . ��� $ SubmenuBorder�� �@��Padding$ 0,3,0,3q��� $ SubmenuBorder��8   � ���� $ SubmenuBorder�� � ��$
 1.25q�P�  � {� .  . ��� 8   � ���� " m�.R��� $��Border��._�� .+��� $��6,3,6,3q�$��Header����.  P�  �@m�IsHighlighted . . ��� $ Border��8    � ���� $ Border�� � ��
 ���� P�  � {� .  . ��� 8   � ���� " m�.R��� $��Border��$ 0,3,0,3q�._��  �� $
��Auto �$
[ Icon���� $��* ��� $
��Auto �$[ Shortcut���� $��13 �.+��� $
��Icon��$��6,0,6,0q�$��Center=�$
��Icon���� $��Check��$��13��$��13��$��6,0,6,0q�$y�	Collapsed=�
������ $��1q����#007edf._��� $��	CheckMark��
���   �$��7��$��7��$y�Hidden=�
 .  �� $��
HeaderHost��$��1��$��Header����. �� $\ InputGestureText��$��2��#��z] $��2,0,2,0q�$��Right=� P�  Z�  �@m�Icon. ��� $
 Icon�� � y�$ Hidden=�P�  �@m�	IsChecked . . ��� $ 	CheckMark�� � y�$ Visible=�P�  �@m�IsCheckable . . ��� $ Check�� � y�$ Visible=��� $
 Icon�� � y�$ Hidden=�P�  �@m�IsHighlighted . . ��� $ Border��8    � ��P�  � {� .  . ��� 8   � ���� " m�.R��� $��Border��$ 0,3,0,3q�._��  �� $
��Auto �$
[ Icon���� $��* ��� $
��Auto �$[ Shortcut���� $��10 �.+��� $
��Icon��$��Center=�$
��Icon���� $��
HeaderHost��$��1��$��Header����. �� $\ InputGestureText��$��2��#��z] $��2,0,2,0q�$��Right=�F� $��3��$��Center=�$��Center=�"����        �    ��j � 	= �g5 	8��  � $��Popup��$��Right=�#��zZ < . |�.  $��Slide=�.���� $��SubmenuBorder��
 . 8�� ._��� A . $B Cycle=� P�  �@m�IsHighlighted . . ��� $ Border��8   � ��P� $C Popup�� �@�AllowsTransparency . . ��� $ SubmenuBorder�� �@��Padding$ 0,3,0,3q��� $ SubmenuBorder��8   � ���� $ SubmenuBorder�� � ��$
 1.25q�P�  � {� .  . ��� 8   � ���� " m�.���  �@�OverridesDefaultStyle . @ P�  �@m�Role$ SubmenuHeader=�. ���  � ��8   P�  �@m�Role$ SubmenuItem=�. ���  � ��8  P�  �@m�Role$ TopLevelHeader=�. ���  � ��8  ��  �@�IsSharedSizeScope . P�  �@m�Role$ TopLevelItem=�. ���  � ��8  �� "  .���  � ��
 �������  � �� �� "  .R��� #��z��#��z��#��z��
 . # z��._��� $��DG_ScrollViewer��|�.  ���� " ��.R�� . �� $
��Auto ��� $��* ��� $
��Auto � �� $
��Auto ��� $��* ��� $
��Auto �.+��� #��Zb |�.  ���� + �� "I  $ * CellsPanelHorizontalOffset���C� *�� "c  $"d DataGridSelectAllButtonStyle��+y��� + �� "I  $* HeadersVisibility�#f Ze #h Zg   $!��PART_ColumnHeadersPresenter��$��1��y��� + �� "I  $* HeadersVisibility�#f Ze #h Zi �� $!j PART_ScrollContentPresenter��$��1��$��2��#k z���� $��PART_VerticalScrollBar��$��1��$��1��$l Vertical=�$��Right=�$��0,0,0,0q�
 . ( . #n zm #p zo #y�zq r �� $* VerticalOffset�+ �� *TemplatedParent+$3 OneWay=����� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . @ P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8   � $��2��$��1�� �� ���� + �� "I  $.* (NonFrozenColumnsViewportHorizontalOffset��� $��* �.+��� $��PART_HorizontalScrollBar��$��1��$l 
Horizontal=�#n zs #p zt #y�zu r �� $* HorizontalOffset�+ �� *TemplatedParent+$3 OneWay=����� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . @ P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  .���� #
 z
 �� "  @ P�  �@ 
IsSelected . . ���  � �� Z� ��  � �� Z� ��  � ��
 �   ��� " n�.���  � ��$ 5����  � ��
 ���� ��  �@�Cursor$ SizeWE����  � �� �� " n�.R��� #��z��# z���� "  .���  � �� �� "  .R�� .+��� �� "  .���  � ��$ Center=���  � ��$ Stretch=���  � ��$ Center=���  � ��
 �   ���  � ��$ 12(���  � ��$
 Bold"���  � ��$ Calibri)���  � �� �� "  J ���� HoverOn5 .��U� $K 00:00:00X�$" 
MainBorder��$# (UIElement.Opacity)�.\��� $L 00:00:00.2000000��$	M 0.6Q��� HoverOff5 .��U� $K 00:00:00X�$" 
MainBorder��$# (UIElement.Opacity)�.\��� $L 00:00:00.3000000��$M 1Q�.R�� .+��� $��
MainBorder��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
�������._�� .+��� $��3q�$��Center=�$��Center=�#��z��#w zv #��z��F� $��UpArrow��
���   �$��Right=�$��Center=�$��0,2,1,0q�$y�Hidden=���B� .��E� $	x 0,0&�.���� $	y 7,0&��� $y 3.5,6&��� $	y 0,0&�F� $��	DownArrow��
���   �$��Right=�$��Center=�$��0,2,1,0q�$y�Hidden=���B� .��E� $	x 0,6&�.���� $	y 7,6&��� $y 3.5,0&��� $	y 0,6&�n� $z PART_LeftHeaderGripper��$��0��$
��Left=�8��  n� $z PART_RightHeaderGripper��$��3��$��Right=�8��  P�  �@ SortDirection$ 
DescendingX�. ��� $ UpArrow�� � y�$ Hidden=��� $ 	DownArrow�� � y�$ Visible=�P�  �@ SortDirection$ 	AscendingX�. ��� $ 	DownArrow�� � y�$ Hidden=��� $ UpArrow�� � y�$ Visible=�P�  �@J�IsMouseOver . . ���  �@�Cursor$
 Hand���� $ 
MainBorder�� � ��
 ������� $ 
MainBorder�� � ��
 �   ���  � �� �#0071C6�� "  .���  � ��$ Center=���  � ��
 �������  � �� �� "  .R�� .+��� $��
MainBorder��$	��0.5q�
���   �8��  ._�� .+��� $��3q�$��Center=�$��Center=�#��z��#w zv #��z���� " ��.���  � �� �� .R��� $D Minimize��$G��A/Agent.Interaction.Desktop;component/Images/Maximize1.Disable.png��$��15��$��11��E �� $
F True��G �� + �� $H FindAncestor=�"I ��$* IsMouseOver�.a��� $ Minimize�� � ��$? 9/Agent.Interaction.Desktop;component/Images/Maximize1.png���� $
F True��G �� + �� $H FindAncestor=�"I ��$* 	IsPressed�.a��� $ Minimize�� � ��$? 9/Agent.Interaction.Desktop;component/Images/Maximize1.png����  � �� �� " ��J ���� OnMouseEnter15 .��U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #
 z
 #��z��z�V� .~��� $N 1Q�$- 1Q��� $O 0Q�$P 0Q��� $ 0Q�T� $Q 0Q�$R 0Q� :� $S Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��.���  � �� �� .R��� $D Minimize��$D��>/Agent.Interaction.Desktop;component/Images/Normal.Disable.png��$��15��$��11��E �� $
F True��G �� + �� $H FindAncestor=�"I ��$* IsMouseOver�.a��� $ Minimize�� � ��$< 6/Agent.Interaction.Desktop;component/Images/Normal.png���� $
F True��G �� + �� $H FindAncestor=�"I ��$* 	IsPressed�.a��� $ Minimize�� � ��$D >/Agent.Interaction.Desktop;component/Images/Normal.Disable.png����  � �� �� " ��J ���� OnMouseEnter15 .��U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�U� $K 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $L 00:00:00��$	M 1.4Q��� $L 00:00:01��$M 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #
 z
 #��z��z�V� .~��� $N 1Q�$- 1Q��� $O 0Q�$P 0Q��� $ 0Q�T� $Q 0Q�$R 0Q� :� $S Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��@ P�  �@��AlternationIndex$ 0��. ���  � ��
 ��·�P�  �@��AlternationIndex$ 1��. ���  � ��
 �����P�  �@��
IsSelected . . ���  � ��
 � ����� .R�]� 
�������.+��� $��10(�
���   �$
��Bold"����� �� " ��.���  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " ��.R��� 
������ �� " n�.���  �@�OverridesDefaultStyle . ��  � �� �� " n�.R�A� ���#DEDEDE���#99BBE8$��15��$��15���� " ��.���  � �� �� " ��.R��� #��z��#��z��#��z��._�� . �� $
��Auto ��� $
��Auto �#��z���� $
��Auto �.+�m� ${ TopTick��$y�	Collapsed=�#| z��$	} Top=�$��4��$��0��m� ${ 
BottomTick��$y�	Collapsed=�#| z��$} Bottom=�$��4��$��0���� $ TrackBackground��8��  8�� $��1q�$T 1��$	��5,0q�$��Center=�$	��4.0��$��1��._��� $��-6,-1q�.+��� $~ PART_SelectionRange��$y�Hidden=�$	��4.0��#��� ��#��� ��$	��1.0��]� $ 
PART_Track��$��1��/ �� 8�� #��Z� 1 �� 8�� #��Z� 0 n� $z Thumb��8�� 
���   ��� " ��.���  � ��$ Center=���  � ��$ Stretch=���  � ��$ Center=���  � ��
 �   ���  � ��$ 12(���  � ��$
 Bold"���  � ��$ Calibri)���  � �� �� " ��J ���� HoverOn5 .��U� $K 00:00:00X�$" 
MainBorder��$# (UIElement.Opacity)�.\��� $L 00:00:00.2000000��$	M 0.6Q��� HoverOff5 .��U� $K 00:00:00X�$" 
MainBorder��$# (UIElement.Opacity)�.\��� $L 00:00:00.3000000��$M 1Q�.R�� .+��� $��
MainBorder��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
�������._�� .+��� $��3q�$��Center=�$��Center=�#��z��#w zv #��z��n� $z PART_LeftHeaderGripper��$��0��$
��Left=�8��  n� $z PART_RightHeaderGripper��$��3��$��Right=�8��  P�  �@J�IsMouseOver . . ���  �@�Cursor$
 Hand���� $ 
MainBorder�� � ��
 ������� $ 
MainBorder�� � ��
 �   ���  � �� �#0071C6