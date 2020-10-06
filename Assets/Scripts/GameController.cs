﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);  // объект на основе структуры, виден только внутри скрипта, тип данных такой же как в структуре 
    public float cubeChangePlaceSpeed = 0.5f; // переменная отвечает за то как быстро новый кубик будет менять свою позицию
    public Transform cubeToPlace; // пеерменная с типом данных 
    private float camMoveToYPosition, camMoveSpeed = 2f;

    public GameObject cubeToCreate, allCubes;
    public GameObject[] canvasStartPage; //массив
    private Rigidbody allCubesRb;

    private bool IsLose, firstCube;

    private List<Vector3> allCubesPositions = new List<Vector3>() //динамический список состоящий из позиций векторов 
    {
        new Vector3(0, 0, 0), // все кооординаты на которых есть определенные кубы
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
    };

    private Transform mainCam;
    private Coroutine showCubePlace;

    private void Start() //функция старт
    {
        mainCam = Camera.main.transform; // переменная с записью к основной камере 
        camMoveToYPosition = 5.9f + nowCube.y - 1f;// значение в саму пременную

        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace()); //функция куратина (постояно менял через какое то время  позицию где можно установить куб)
    }

    private void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null  && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
            if(!firstCube) //нажатие на экран и все элементы скрыть 
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj); //удаление при запуске игры
            }

           GameObject newCube = Instantiate(
                cubeToCreate,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;

            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(nowCube.getVector());

            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;

            SpawnPosition();
            MoveCameraChangeBg(); // проверяем какоц у нас сейчас максимальный кубик и передвигать камеру
        }

        if(!IsLose && allCubesRb.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);

        }
        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z),
            camMoveSpeed * Time.deltaTime);
    }

    IEnumerator ShowCubePlace() // куратина
    {
        while (true) //бесконечный цикл
        {
            SpawnPosition(); //вызывается отдельная функция 
            yield return new WaitForSeconds(cubeChangePlaceSpeed); //вызов функции через определенные промежутки времени
        }
    }

    private void SpawnPosition() //метод
    {
        List<Vector3> positions = new List<Vector3>(); //класс (динамичский список или динамический массив данных) внутри спика positions все доступные позиции где расположиться CubeToPlace
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z))
            && nowCube.x + 1 != cubeToPlace.position.x) //динамический массив (в функцию передается позицию которую нужно проверить,
        {
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)); 
        }
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z))
            && nowCube.x - 1 != cubeToPlace.position.x) 
        {
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z))
            && nowCube.y + 1 != cubeToPlace.position.y)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z))
             && nowCube.y - 1 != cubeToPlace.position.y)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1))
             && nowCube.z + 1 != cubeToPlace.position.z)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1))
             && nowCube.z - 1 != cubeToPlace.position.z)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        }

        cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)]; // обратимся к списку для случайно позиции
        
    }

    private bool IsPositionEmpty(Vector3 targetPos) // функция будет возвращать true если место является свободным и false если место занято
    {
        if (targetPos.y == 0) 
            return false;
        foreach (Vector3 pos in allCubesPositions) // перебирать
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
                return false;
        }
        return true;
    }

    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0; //список allCebesPositions перебираем и находим максимальный элемент по каждой кардинате 

        foreach(Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX) // нахождение максимального элемента по координат x
                maxX = Convert.ToInt32(pos.x);

            if (Convert.ToInt32(pos.y) > maxY)
                maxY = Convert.ToInt32(pos.y);

            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }
        camMoveToYPosition = 5.9f + nowCube.y - 1f;
    }

}

struct CubePos // отвечает за хранение кординат обЪекта
{
    public int x, y, z;
    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector()
    {
        return new Vector3(x, y, z);  //возвращение вектора состоящих из координат 
    }

    public void setVector(Vector3 pos)  //метод принимает вектор
    {
        x = Convert.ToInt32(pos.x);  //преобразование типов из float в int
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}
