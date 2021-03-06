﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using Autodesk.AutoCAD.EditorInput;
using acad = Autodesk.AutoCAD.ApplicationServices;

namespace ACADLib.Models
{
    public class Circles : Objects
    {


        #region Properties

        private Point3d _circleCenter;
        /// <summary>
        /// Координаты центра окружности
        /// </summary>
        public Point3d CircleCenter
        {
            get { return _circleCenter; }
            set { _circleCenter = value; }
        }

        private double _circleRadius;
        /// <summary>
        /// Радиус окружности
        /// </summary>
        public double CircleRadius
        {
            get { return _circleRadius; }
            set { _circleRadius = value; }
        }


        public ObjectId CircleID;
        #endregion


        /// <summary>
        /// Выделить с экрана окружность
        /// </summary>
        public void GetCircle()
        {
            var acDocE = Application.DocumentManager.MdiActiveDocument.Editor;
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                var promtResult = acDocE.GetEntity("Выберите Объект");

                try
                {
                    if (promtResult.Status == PromptStatus.OK)
                    {
                        Circle acEnt = acTrans.GetObject(promtResult.ObjectId, OpenMode.ForRead) as Circle;

                        //Записываем нужные аттрибуты в глобальные переменные
                        _circleCenter = acEnt.Center;
                        _circleRadius = acEnt.Radius;
                        CircleID = acEnt.ObjectId;
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception Ex)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Error:\n" + Ex.Message);
                }

                //Закрываем транзакцию
                acTrans.Commit();
            }
        }

 
        /// <summary>
        /// Добавление окружности с заданными параметрами 
        /// </summary>
        /// <param name="point3d"></param>
        /// <param name="radius"></param>
        [CommandMethod("AddCircle")]
        public void AddCircle(Point3d point3d, Double radius)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            using (DocumentLock docLock = acDoc.LockDocument())
            {
                Database acCurDb = acDoc.Database;

                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                    OpenMode.ForWrite) as BlockTableRecord;
                    {
                        // добавляем окружность
                        Circle acCircle = new Circle();

                        // устанавливаем параметры созданного объекта
                        acCircle.SetDatabaseDefaults();
                        acCircle.Center = point3d;
                        acCircle.Radius = radius;

                        // добавляем созданный объект в пространство модели и в транзакцию
                        acBlkTblRec.AppendEntity(acCircle);
                        acTrans.AddNewlyCreatedDBObject(acCircle, true);

                    }
                    // фиксируем изменения
                    acTrans.Commit();
                }
            }
        }
        
    }
}
