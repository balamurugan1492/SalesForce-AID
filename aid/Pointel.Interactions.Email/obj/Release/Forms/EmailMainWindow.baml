   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  U QPointel.Interactions.Email, Version=5.0.3.6, Culture=neutral, PublicKeyToken=nulle7clr-namespace:Pointel.Interactions.Email.CustomControls)Pointel.Interactions.Email.CustomControls ]3clr-namespace:Pointel.Interactions.Email.Converters%Pointel.Interactions.Email.Converters 6   0Pointel.Interactions.Email.Forms.EmailMainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     6sys&clr-namespace:System;assembly=mscorlib     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     QCustomControl7clr-namespace:Pointel.Interactions.Email.CustomControls     6my-http://schemas.microsoft.com/wpf/2008/toolkit  Aconvert3clr-namespace:Pointel.Interactions.Email.Converters  -   $��EmailWindow��  � Title$  Email��$
��Auto��$
��Auto��$	��480�� � AllowsTransparency .  � WindowStyle$
 None=� � 
ResizeMode$ 	CanResize=�
������  � Icon$@ :/Agent.Interaction.Desktop;component/Images/Email/Send.png�� � 	Resources  �� MergedDictionaries �� SourceH  BPointel.Interactions.Email.Converters.StringtoImageSourceConverter   StringToImageSourceConvert��  �� $S M/Pointel.Interactions.Email;component/Resources/StylesResourceDictionary.xaml?�%   &	          .��� -   $��MainGrid��.+��� -   $��
MainBorder��$
��0.89q�$��8q�
�������._��  � RowDefinitions �� $
��Auto ��� $��* �.+��� -   $��RightSideRect��$��2��
������ $��3��	 K� ZIndex$
	 1000��$��Right=�$��Stretch=��� -   $��RightbottomSideRect��$��1��
������ $��8��$��8��$
	 2200��$��Right=�$��Bottom=��� -   $��BottomSideRect��$��1��
������ $��3��$
	 1000��$��Stretch=�$��Bottom=�� $��0��
 � ColumnDefinitions
 �� $
��Auto ��� $��* ��� $
��Auto �.+��� -   $��agentStateBorder��$��0��$��5,0,0,0q�
�������$��60��._���  �� Orientation$ 
Horizontal=�.+��� -   $��imgEmail��$��12��$��Center=�$C��=/Pointel.Interactions.Email;component/Images/E-Mail/Email.png��$��1q��� -	   $��1��$��18�����#007EDF$	��Top=�$��0,-1,0,0q�._�� 
 �� -
    �Name$ grdTitle��$��* ��� $
��Auto �.+��� -   $��lblTitleStatus��$��0��$��Stretch=�$��Calibri)�$��14(�
�������$
��Bold"�$��Stretch=�$��5,0,10,0q�$��CharacterEllipsis=�����  �� ElementName$ grdTitle�� �� Path$ Width��� -   $��txtEmailStatus��$��1��$��Right=�$��5,0,5,0q�$��Calibri)�$��14(�
�������$
��Bold"�$��	AID Email��$y�Hidden=��� $��2��$��60��$
��Auto��$��Right=�$	��Top=�$ 
Horizontal=�.+��� -   $��btnMinimize��$��16��$��20��  MinimizeButton#��[ �� -   $��btnMaximize��$��16��$��20��  maximizeButton#��[ �� -   $��btnExit��$��16��$��20��  CloseButton#��[ � $��1�� �� $��* �$	��440��
 �� -   $ colMain��$��* �$	��380���� -   $ colEmailOptions��$��19 ��� -   $ colDataResHistory��$	��380 �$	��380��.+�� -   $��grdEmailMain��$��0��
 �� $��* �.+��� -   $��tabctlEmail��
�������$��Stretch=�$��2q�$	��450��$��0��.@��� -   $��tabItemEmail����� $��25��$
��Auto��$��Center=�$��0,2,0,0q�
 �� -   $ tabHeaderRow��$��* ��� $
��Auto ��� $
��Auto �.+��� -   $��lbltabCustomerName��$��0��$��CharacterEllipsis=�$��Center=�$��Stretch=����#0071C6$
��Bold"�$��3,0,3,0q����� $ tabHeaderRow��$ Width��� -   $��imgEmailIcon��$��1��$C��=/Pointel.Interactions.Email;component/Images/E-Mail/Email.png��$��16��$��16��$��Center=�$��2,2,0,2q�:  4Pointel.Interactions.Email.CustomControls.TimerLabel  -     Name$ lblTabItemShowTimer��$��2��   Text$ 
[00:00:00]��$
��Left=�$��Center=�$��61��$��5,3,3,3q����#0071C6$
��Bold"�.��� 
������ $��0q�
 �� $��* � �� -   $ grdrow_error��$��0 ��� -   $ EmailToolBar��$
��Auto ��� -   $ Recentinteractions��$
��Auto ��� -   $ EmailContent��$��* �.+�� $
��Auto��$
��Auto��$��0��
�������
 �� $��30 ��� -   $ grdErrorColumn��$��* ��� $��30 �.+��� $C��=/Agent.Interaction.Desktop;component/Images/Contact/Error.png��$��0��$��5q�$��15��$��15���� -   $��	lblErrMsg��$��1��$��WrapWithOverflow=�$��Center=�$��5q��� -    $��btnCloseError��$��Right=�$��2��$��20��$��20��  NormalImageButton#��[ $��Center=�$��4q�.���� $H��B/Agent.Interaction.Desktop;component/Images/Contact/blue-close.png��� $��1��$��2,5,2,2q�
 �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -!   $��panelInbound��$ 
Horizontal=�$y�Visible=�$��2q�$��0��.+��� -"   $��btnReply��$��45��$��55��$��55��$��2,0,0,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $I��C/Pointel.Interactions.Email;component/Images/E-Mail/Email Reply.png��$��45��$��45��$��	0,-09,0,0q��� $��Center=� �� TextAlignment$ Center=�$��Reply��$��Calibri)�$��11(�$
��Wrap=�$��	2,1.5,2,0q� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -#   $��ReplyHeading��$��Reply��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -$   $��ReplyContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$+��%Reply the email interaction to sender���� -%   $��btnReplyAll��$��45��$��55��$��55��$��2,0,0,0q�#��[ $	��Top=�$��Stretch=�$y�Visible=�.���� $ Vertical=�$��Center=�.+��� $M��G/Pointel.Interactions.Email;component/Images/E-Mail/Email Reply all.png��$��45��$��45��$��	0,-10,0,0q��� $��Center=�$ Center=�$��	Reply All��$��Calibri)�$��11(�$
��Wrap=�$��	2,2.5,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -&   $��ReplyAllHeading��$��	Reply All��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -'   $��ReplyAllContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$@��:Reply the email interaction to sender and other recipients���� -(   $��btnInboundSave��$��45��$��57��$��55��$��-2,0,0,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $I��C/Pointel.Interactions.Email;component/Images/E-Mail/Email save1.png��$��45��$��44��$��0,-6,0,0q��� $��Center=�$ Center=�$
��Save��$��Calibri)�$��11(�$
��Wrap=�$��
2,-0.5,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -)   $��InboundSaveHeading��$
��Save��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -*   $��InboundSaveContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$�� ���� -+   $��btnTransfer��$��45��$��55��$��55��$��-4,0,0,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $L��F/Pointel.Interactions.Email;component/Images/E-Mail/Email transfer.png��$��45��$��45��$��	0,-09,0,0q��� $��Center=�$ Center=�$��Transfer��$��Calibri)�$��11(�$
��Wrap=�$��	2,1.3,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -,   $��TransferHeading��$��Transfer��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� --   $��TransferContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$3��-Transfer the email to another internal target���� -.   $��
btnForward��$��45��$��55��$��55��$��3,0,0,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $K��E/Pointel.Interactions.Email;component/Images/E-Mail/Email Forward.png��$��45��$��45��$��	0,-10,0,0q��� $��Center=�$ Center=�$��Forward��$��Calibri)�$��11(�$
��Wrap=�$��	2,2.3,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -/   $��ForwardHeading��$��Forward��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -0   $��ForwardContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$,��&Forward the email to external resource���� -1   $��btnMarkDone��$��45��$��62��$��55��$��	0,0,1.5,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $H��B/Pointel.Interactions.Email;component/Images/E-Mail/Email Done.png��$��45��$��45��$��	0,-10,0,0q��� $��Center=�$ Center=�$��	Mark Done��$��Calibri)�$��11(�$
��Wrap=�$��	2,2.3,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -2   $��MarkDoneHeading��$��	Mark Done��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -3   $��MarkDoneContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$ ��Done the email interaction���� -4   $��panelOutbound��$ 
Horizontal=�$y�Visible=�$��2q�$��1��.+��� -5   $��btnSend��$��45��$��55��$��55��$��2,0,0,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $H��B/Pointel.Interactions.Email;component/Images/E-Mail/Email Send.png��$��45��$��45��$��	0,-09,0,0q��� $��Center=�$ Center=�$
��Send��$��Calibri)�$��11(�$
��Wrap=�$��2,2,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -6   $��SendHeading��$
��Send��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -7   $��SendContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$0��*Sends email to sender and other recipients���� -8   $��btnOutboundSave��$��45��$��55��$��55��$��-2,0,0,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $I��C/Pointel.Interactions.Email;component/Images/E-Mail/Email save1.png��$��45��$
��42.5��$��
0,-5.5,0,0q��� $��Center=�$ Center=�$
��Save��$��Calibri)�$��11(�$
��Wrap=�$��	2,0.5,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -9   $��OutboundSaveHeading��$
��Save��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -:   $��OutboundSaveContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$�� ���� -;   $��btnAttachment��$��50��$��60��$��55��$��-6,0,0,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $H��B/Pointel.Interactions.Email;component/Images/E-Mail/Attachment.png��$��35��$��34���� $��Center=�$ Center=�$��
Attachment��$��Calibri)�$��11(�$
��Wrap=�$��	2,3.5,2,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -<   $��AttachmentHeading��$��
Attachment��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -=   $��AttachmentContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$.��(Add Attachments to the email interaction���� ->   $��	btnDelete��$��45��$��55��$��55��$��2,0,5,0q�#��[ $	��Top=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $J��D/Pointel.Interactions.Email;component/Images/E-Mail/Email delete.png��$��45��$��45��$��	0,-09,0,0q��� $��Center=�$ Center=�$��Delete��$��Calibri)�$��11(�$
��Wrap=�$��	0,1.5,5,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -?   $��DeleteHeading��$��Delete��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -@   $��DeleteContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$"��Delete the email interaction���� $ 
Horizontal=�$��2q�$��2��.+��� -A   $��btnConsultCall��$��45��$��55��$��55��$��	-10,0,0,0q�#��[ $	��Top=�$��Stretch=�{��� *IsConsultEnabled+.���� $ Vertical=�$��Center=�.+��� $��35��$��33��$��0,2,0,2q�����  �� 
TargetType" �� �� Triggers ��  �� Binding �� $ 	IsEnabled�$ btnConsultCall�� �� Value True.a���  �� Property � �� �� Value$O I/Pointel.Interactions.Email;component/Images/E-Mail/Chat.Consult.Call.png����  �� $ 	IsEnabled�$ btnConsultCall��	 False.a���  � ��$W Q/Pointel.Interactions.Email;component/Images/E-Mail/Chat.Consult.Call.Disable.png���� $��Center=�$ Center=�$��Consult��$��Calibri)�$��11(�$
��Wrap=�$��	2,0.3,0,0q� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -B   $��ConsultCallHeading��$��Consult call��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -C   $��ConsultCallContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$7��1Consult with an internal target or with a contact���� $��1q����#D6D7D6$��2q�$��2��._�� 
 �� $��* ��� $
��Auto ��� $
��Auto ��� $
��Auto � �� $��30 �.+�� $��0��$��0,2,0,0q�
 �� $
��Auto ��� -D   $ gcCustomerDetail��$��* ��� $
��Auto ��� $
��Auto �.+��� -E   $��btnCustomerDetails��$��0��$��15��#��[  � Cursor$
 Hand��$��0q�$
��Left=�$��5,0,5,0q�{��� $ 	IsEnabled�$ btnConsultCall��.���� $C��=/Pointel.Interactions.Email;component/Images/E-Mail/Arrow.png��$
��Fill=�$��10��$��10��$��Bottom=��� -F   $��lblCustomerName��$��1��$��Center=�$��5,0,10,0q�$��Stretch=�$��CharacterEllipsis=����� $ gcCustomerDetail��$ Width��� -G   $��imgEmailStatusIcon��$��2��$C��=/Pointel.Interactions.Email;component/Images/E-Mail/Email.png��$��18��$��18���� -H   $��lblEmailIxnType��$��3��$��Center=�$��5,0,5,0q��� -I   $��btnPrint��$��1��$��2,0,2,0q�#��[ $��Center=�$��Stretch=�.���� $ Vertical=�$��Center=�.+��� $C��=/Pointel.Interactions.Email;component/Images/E-Mail/Print.png��$��22��$��22�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -J   $��PrintHeading��$��Print��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -K   $��PrintContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$!��Print the email interaction���� -L   $��borderInprogress��$��2q�$��2��$��1q�$	��Top=�y��� *InProgressIXNCountVisibility+._��� $ 
Horizontal=�$��Center=�y��� *InProgressIXNCountVisibility+.+��� -M   $��imgInteractionsStatus��$
��Left=�$N��H/Pointel.Interactions.Email;component/Images/E-Mail/IxnInProgrogress.png��$
��Left=�$��LeftToRight=�$��Center=�$��18��$��18��$��2q��� -N   $��lblInprogressIxnCount��$
��Left=�$��Center=����� *InprogressInteractionCount+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -O   $��IXNCountHeading��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *TotalInprogessIXNCount+�� -P   $��stkPanelIXNCountContent��$��
10,5,10,10q�$ Vertical=�$��1��$
��Auto��$	��200������ " �� P�  P� Property �@J�IsMouseOver P� Value . . ���  � �� �#939589��  � �� �#CEE7F7�� -Q   $��borderRecentIXN��$��2,2,5,2q�$��3��$��1q�$��0��$��1��$	��Top=�y��� *RecentIXNListVisibility+._��� $ 
Horizontal=�$��Center=�y��� *RecentIXNListVisibility+.+��� -R   $��imgInteractionsList��$
��Left=�$K��E/Pointel.Interactions.Email;component/Images/E-Mail/RecentIxnList.png��$
��Left=�$��LeftToRight=�$��Center=�$��18��$��18��$��2q��� -S   $��lblInteractionList��$
��Left=�$��Center=����� *RecentIXNCount+���� " �� P�  �@J�IsMouseOver . . ���  � �� �#939589��  � �� �#CEE7F7� -T   $��dockEmailContent��$��3��$��2q�� $��1��$��2,0,2,0q� �� $
��Auto ��� .+��� -U   $��btnShowHideIXNPanel��$��0��$��0��$
��Auto��#��[ $	��Top=�$	��Top=�.���� $ Vertical=�$��Center=�.+��� -V   $��imgShowHideIXNPanel��$?��9/Agent.Interaction.Desktop;component/Images/Hide_Left.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -W   $��ToolHeading��$
��Hide��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -X   $��ToolContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$*��$Agent can hide the interaction panel��A�  A� Rows$ 3�� A� Columns$ 1��$��1��$��0��.+�d� -Y   $��btnData��$
 Hand��  
SideButton#��[ $��0,0,0,1q�.��� .+��� -Z   $��imgData��$��18��$��10��$��Right=����� *CasedataImageSource+�� $
��DATA��$��9(�$
��Wrap=�
�������d� -[   $��btnContacts��$
 Hand��#��[ $��0,0,0,1q�.��� .+��� -\   $��imgContacts��$��18��$��10��$��Right=����� *ContactImageSource+�� $��CONTACT��$��9(�$
��Wrap=�
�������d� -]   $��btnResponses��$
 Hand��#��[ $��0,0,0,1q�.��� .+��� -^   $��imgResponse��$��18��$��10������ *ResponseImageSource+�� $��	RESPONSES��$��9(�$
��Wrap=�
�������� -_   $��gridDataResHistory��$��0,2,2,2q�$��2��$��Stretch=�� -`   $��popupRecentIXNDetails��$��Bottom=� � 	StaysOpen .    � AllowsTransparency  . $��Bottom=�.���� ���#ADAAAD$��1q�$��2q�$
��Auto��$
��Auto��
�������._�� 
 �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -a   $��lblRecentIXNCount��$��2��$��0��$
��Left=�$��5q����� *RecentInteractionCount+)  #Microsoft.Windows.Controls.DataGrid  -b   $��DGRecentInteraction��!   RowHeaderWidth$! 0Q�
������ $��5q�
�������$
��Left=�$��Center=�$
��Left=�"   HorizontalGridLinesBrush
" ���� #   VerticalGridLinesBrush
# ���� $   AutoGenerateColumns$ .  %   CanUserAddRows% .  &   CanUserDeleteRows& .  $��2��$��1��'   HeadersVisibility$
' None=����� *RecentInteraction+(   Columns( 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /  )Microsoft.Windows.Controls.DataGridColumn)   Header$
) Icon��*   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
* Auto +   MinWidth$+ 30Q�,   CellTemplate, �� .R��� $��
imgIXNIcon��$
��Left=�$��2,0,2,0q�$��15��$��15������ $ 	MediaType�- �� 	Converter#- [    $) Started Date��$
* Auto , �� .R��� $��txtIXNStartDate��$
��Left=�$��2,0,2,0q����� *InteractionStarted+  $) Subject��$
* Auto , �� .R��� $��txtIXNSubject��$
��Left=�$��2,0,2,0q����� *InteractionSubject+.   RowStyle. �� ,  &Microsoft.Windows.Controls.DataGridRow"   P�  �@ 
IsSelected . . ���  � ��  HighlightedBrush# [ �� -c   $��lblRemainingIXN��$��2��$��2��$
��Left=�$��5q����� *RemainingDetails+� -d   $��popupAgentInfo��$��Bottom=� .    . $��Bottom=�.���� ���#ADAAAD$��1q�$��10q�$
��Auto��$
��Auto��
�������._�� 
 �� $
��Auto ��� $
��Auto � �� $
��Auto ��� $
��Auto �.+��� $ 
Horizontal=�$��2��$��0��$
��Left=�$��5q�.+��� -e   $��lblagentUserName��$��Italic$�$��User Name: ��$��5,0,0,0q��� -f   $��lblagentName��$��Normal$�$��5,0,0,0q����� *
UserName+�� $ 
Horizontal=�$��2��$��1��$
��Left=�$��5q�.+��� -g   $��lblagentPlace��$��Italic$�$��Using Place: ��$��5,0,0,0q��� -h   $��lblPlace��$��Normal$�$��5,0,0,0q����� *	PlaceID+