   M S B A M L   `   `   `  ���� E  ARootFinder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:RootFinder
RootFinder  "    RootFinder.ChartConfigWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     *localclr-namespace:RootFinder     -     � Title$  ChartConfigWindow��$��269.82��$��433.94�� � 
ResizeMode$ NoResize=� � 	Resources $  RootFinder.RangeValueConverter   RangeValueConverter'  !RootFinder.AccuracyValueConverter  AccuracyValueConverter��%   &	        &	          .��� $��0,0,2,-1q�.+��� $3��-Here you can configure some chart properties:��$��10q�.��� .+��� $��	Accuracy:��$
��Left=�$��	10,20,0,0q�$	��Top=��� $��
X - Range:��$
��Left=�$��	10,51,0,0q�$	��Top=��� $��
Y - Range:��$
��Left=�$��	10,82,0,0q�$	��Top=��� -   $��OK��$
��Left=�$��309,169,0,0q�$	��Top=�$��75���� -   $��Cancel��$
��Left=�$��203,169,0,0q�$	��Top=�$��75���� $��
136,20,0,0q� �� Orientation$ 
Horizontal=�$
��Left=�$	��262��$��32��$	��Top=�.+��� $	��Low��$
��Left=�$	��Top=��� -    ��Name$ sldrAcc��$
��Left=�$��0,4,0,0q�$	��Top=�$��97�� � Minimum$ 1Q� � Maximum$ 8Q� � LargeChange$ 1Q� � SmallChange$ 1Q�	 � Value$	 4Q��� $
��High��$
��Left=�$��0,4,0,0q�$	��Top=��� $
��Left=�$��0,4,0,0q�$	��Top=�$��92������ *Value+
 �� 	Converter#
 [  �� ElementName$ sldrAcc�� �� IsAsync . �� $��
136,55,0,0q�$ 
Horizontal=�$
��Left=�$	��228��$��28��$	��Top=�.+��� $��From:��$
��Left=�$	��Top=�$��40���� -    ��Name$ txtXFrom��$
��Left=�$��23��$��0,3,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$��60������ *Text+#
 [  $ txtXTo���� $	��To:��$
��Left=�$	��Top=�$��40��$��Right=��� -   $ txtXTo��$
��Left=�$��23��$��0,3,0,0q�$
 Wrap=�$	��Top=�$��60������ *Text+#
 [  $ txtXFrom���� -    ��Name$ stackYRange��$��
136,81,0,0q�$ 
Horizontal=�$
��Left=�$	��228��$��28��$	��Top=�.+��� $��From:��$
��Left=�$	��Top=�$��40���� -   $ txtYFrom��$
��Left=�$��23��$��0,3,0,0q�$
 Wrap=�$	��Top=�$��60������ *Text+$ txtYTo��#
 [  �� $	��To:��$
��Left=�$	��Top=�$��40��$��Right=��� -	   $ txtYTo��$
��Left=�$��23��$��0,3,0,0q�$
 Wrap=�$	��Top=�$��60������ *Text+$ txtYFrom��#
 [  �� -
    ��Name$ 
chkBindYFx��$ ��Bind Y-Range to F(x) range��$
��Left=�$��
10,124,0,0q�$	��Top=��� -   $ 
chkBindYGx��$ ��Bind Y-Range to G(x) range��$
��Left=�$��
10,145,0,0q�$	��Top=�