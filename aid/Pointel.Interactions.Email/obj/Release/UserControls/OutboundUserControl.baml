   M S B A M L   `   `   `  ����   Pointel.Interactions.EmailA    ;Pointel.Interactions.Email.UserControls.OutboundUserControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     I EWpfAnimatedGif, Version=1.4.4.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  :d2http://schemas.microsoft.com/expression/blend/2008  -     � 	Resources   �� MergedDictionaries �� Source��  �� $S M/Pointel.Interactions.Email;component/Resources/StylesResourceDictionary.xaml?�%    .���� -   $��
MainBorder��$
��0.89q�$��2q�
�������._��  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+���  �� Orientation$ Vertical=�.+�� $��2q� � ColumnDefinitions �� $��50 ��� $��* �.+��� $��Center=�$��50��.��From :�� $��Stretch=�$��1��.+��� -   $��cmbFromAddress��$��5,0,5,0q�$��25�� �� 
IsEditable .  $��Stretch=�$y�	Collapsed=��� -   $��txtOutboundFrom��$
��Auto��$��Center=�$��5,0,5,0q��� -   $��stkFromAddressError��$��Right=�$ 
Horizontal=�$y�Visible=�.+��� $��Addresses are populating��$��Italic$����#df4e00�� -   $��
ImgPreload�� K� ZIndex$ 0��$��12��$��12��$��Center=�$��Center=�"  WpfAnimatedGif.ImageBehavior   AnimatedSource$> 8/Agent.Interaction.Desktop;component/Images/Loading1.gif��� -   $��gridTo��$��2q� �� $��50 ��� $��* ��� -   	 �Name$	 colAddCcBcc��$
��Auto �.+��� -	   $��btnOutboundTo��$��0��$��25��$��50��$��	10,2,10,2q�$
��Left=�   NormalButton#��[  $
��To..��$��Normal"��� -
   $��txtOutboundTo��
 �� 	UndoLimit$
 10��$��1��$
��Auto��$��22�� �� TextWrapping$ NoWrap=�$��Center=�$��5,0,5,0q�  DefaultCopyContextmenu � ContextMenu# [ �� $��2��$ 
Horizontal=�.+��� -   $��btnAddCcandBcc��$��Stretch=�$��25��$
��Auto��$��2q�$��Right=�$��Add Cc��$��Normal"�#��[  �� -   $��btnMenuCCBcc��$��15��  NormalImageButton#��[  � Cursor$
 Hand��$��0q�.���� $C��=/Pointel.Interactions.Email;component/Images/E-Mail/Arrow.png��$
��Fill=�$��10��$��10��$��Bottom=�� -   $��gridCc��$��2q� �� $��50 ��� $��* ��� $��20 �.+��� -   $��btnOutboundCc��$��0��$��Stretch=�$��25��$��50��$��	10,2,10,2q�$
��Left=�#��[  $
��Cc..��$��Normal"��� -   $��txtOutboundCc��$
 10��$��1��$
��Auto��$��22��$ NoWrap=�$��Center=�$��5,0,2,0q�# [ �� -   $��btnCcDelete��$��2��#��[ $��Center=�$��Center=�$��15��$��15��.���� $ Vertical=�$��Center=�.+��� $D��>/Pointel.Interactions.Email;component/Images/E-Mail/Delete.png��$��14��$��14�� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��	Remove Cc��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)�� -   $��gridBcc��$��2q� �� $��50 ��� $��* ��� $��20 �.+��� -   $��btnOutboundBcc��$��0��$
��Left=�$��Stretch=�$��25��$��50��$��	10,2,10,2q�#��[  $��Bcc..��$��Normal"��� -   $��txtOutboundBcc��$
 10��$��1��$��22��$
��Auto��$ NoWrap=�$��Center=�$��5,0,5,0q�# [ �� -   $��btnBccDelete��$��2��#��[ $��Center=�$��Center=�$��15��$��15��.���� $ Vertical=�$��Center=�.+��� $D��>/Pointel.Interactions.Email;component/Images/E-Mail/Delete.png��$��14��$��14�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��
Remove Bcc��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)�� -   $��gridSubject��$��2q� �� $��50 ��� $��* �.+��� $��Center=�$��51��.��	Subject :�� -   $��txtOutboundSubject��$
 10��$��1��$��22��$
��Auto��$ NoWrap=�$��Center=�$��5,0,5,0q�# [ �  �� -   $	 colAttachError��$��25 ��� $��* �.+��� ���#FFFFCC.+��� $C��=/Agent.Interaction.Desktop;component/Images/Contact/Error.png��$��0��$��5q�$��15��$��15��$	��Top=��� -   $��	attscroll��$��1��$��60����. $��Hidden=�$
��Auto=�.��� -   $��dockOutboundAttachments��$
��Auto=�]� -   $��dockOutboundContent��$��1��9� -   $��expOrginalEmail��$��2�� ' #StretchyExpanderTempforOrginalEmail#��[  � OverridesDefaultStyle . $��Orginal Email��$��Stretch=�$	��Top=�$y�	Collapsed=�.��]� -   $��dpOrginalEmail��$	��100��