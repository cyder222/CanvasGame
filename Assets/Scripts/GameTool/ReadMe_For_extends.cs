/*
どうやって、ゲームを拡張していくか

#ツールの追加について

ゲームで、設置できるツールを追加するには以下の手順を踏む

1.Unity上に必要な画像などを配置して、生成するオブジェクトのプリファブを作成
2. Script/ObjectCreator.csのUpArrowTools , GroundLineToolを参考にしながら、新しい生成の処理を書く
3. Script.ObjectCreator ObjectType列挙体にタイプを追加
4. PhysicsObjectCreatorのcreatePrimitiveメソッドに、プリファブの生成処理を追加
5. CurrentToolクラスのOnChangeメソッドを変更する＋アニメーターファイルを編集して、
   ツール画像が切り替わるようにする。アニメーターファイルは、Unityエディタ
   のToolCanvas→selectedTool→Imageにセットしてある。

#ステージの追加について
将来的に対応

 * **/