/*
 * Copyright (c) 2020 Razeware LLC
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish,
 * distribute, sublicense, create a derivative work, and/or sell copies of the
 * Software in any work that is designed, intended, or marketed for pedagogical or
 * instructional purposes related to programming, coding, application development,
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works,
 * or sale is expressly withheld.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public class WaveGenerator : MonoBehaviour {
    [Header("Wave Parameters")] public float waveScale;
    public float waveOffsetSpeed;
    public float waveHeight;

    [Header("References and Prefabs")] public MeshFilter waterMeshFilter;
    private Mesh waterMesh;

    NativeArray<Vector3> waterVertices;
    NativeArray<Vector3> waterNormals;

    JobHandle meshModificationJobHandle; // 1
    UpdateMeshJob meshModificationJob; // 2

    private void Start() {
        waterMesh = waterMeshFilter.mesh;

        waterMesh.MarkDynamic(); // 1

        waterVertices =
            new NativeArray<Vector3>(waterMesh.vertices, Allocator.Persistent); // 2

        waterNormals =
            new NativeArray<Vector3>(waterMesh.normals, Allocator.Persistent);
    }

    private void Update() {
        meshModificationJob = new UpdateMeshJob() {
            vertices = waterVertices,
            normals = waterNormals,
            offsetSpeed = waveOffsetSpeed,
            time = Time.time,
            scale = waveScale,
            height = waveHeight
        }; // 1 


        meshModificationJobHandle =
            meshModificationJob.Schedule(waterVertices.Length, 64); // 2
    }

    private void LateUpdate() {
        meshModificationJobHandle.Complete(); //1

        waterMesh.SetVertices(meshModificationJob.vertices); // 2

        waterMesh.RecalculateNormals(); // 3
    }

    private void OnDestroy() {
        waterVertices.Dispose();
        waterNormals.Dispose();
    }

    [BurstCompile]
    private struct UpdateMeshJob : IJobParallelFor {
        public NativeArray<Vector3> vertices; //1

        [ReadOnly] public NativeArray<Vector3> normals; //2

        public float offsetSpeed; //3
        public float scale;
        public float height;

        public float time; //4

        private float Noise(float x, float y) {
            float2 pos = math.float2(x, y);
            return noise.snoise(pos);
        }

        public void Execute(int i) {
            // 1
            if (normals[i].z > 0f) {
                // 2
                var vertex = vertices[i];

                // 3
                float noiseValue =
                    Noise(vertex.x * scale + offsetSpeed * time, vertex.y * scale +
                                                                 offsetSpeed * time);

                // 4
                vertices[i] =
                    new Vector3(vertex.x, vertex.y, noiseValue * height + 0.3f);
            }
        }
    }
}