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

    public GameObject cubeToCreate, allCubes;
    public GameObject[] canvasStartPage;
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

    private Coroutine showCubePlace;

    private void Start() //функция старт
    {
        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace()); //функция куратина (постояно менял через какое то время  позицию где можно установить куб)
    }

    private void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
            if(!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);
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
        }

        if(!IsLose && allCubesRb.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);

        }
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
