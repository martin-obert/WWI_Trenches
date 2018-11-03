﻿using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.JobTests
{
  

    [Serializable]
    public struct Group : ISharedComponentData
    {
        public int Id;
    }
}