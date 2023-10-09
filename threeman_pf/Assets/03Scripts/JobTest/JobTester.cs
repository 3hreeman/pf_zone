using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JobTester : MonoBehaviour {
public enum TestType{
    None,
    Normal,
    Job,
    Burst
}
    public int count = 1000000;
    private float[] testValues;

    public TestType testType = TestType.None;
    public bool testRun = false;

    private void Start() {
        testRun = false;
        testType = TestType.None;
        testValues = new float[count];
        for( int i = 0; i < count; i++ ) {
            testValues[i] = 0;
        }
    }

    void Update() {
        if(!testRun) return;

        if (testType == TestType.Normal) {
            JustUpdate();
        }
        else if (testType == TestType.Job) {
            JobUpdate();
        }else if (testType == TestType.Burst) {
            BurstUpdate();
        }
        
    }

    void JustUpdate() {
        for( int i = 0; i < count; i++ )
        {
            testValues[i] += math.sin( Time.deltaTime );
        }

        Debug.LogWarning( "0 Index : " + testValues[0] );
    }

    void JobUpdate() {
        NativeArray<float> results = new NativeArray<float>( testValues, Allocator.TempJob );

        JobStruct testJob = new JobStruct();
        testJob.deltaTime = Time.deltaTime;
        testJob.result = results;
        JobHandle handle = testJob.Schedule( results.Length, 32 );

        handle.Complete();
        results.CopyTo( testValues );
        results.Dispose();

        Debug.LogWarning( "0 Index : " + testValues[0] );
    }
    
    void BurstUpdate() {
        NativeArray<float> results = new NativeArray<float>( testValues, Allocator.TempJob );

        BurstStruct testJob = new BurstStruct();
        testJob.deltaTime = Time.deltaTime;
        testJob.result = results;
        JobHandle handle = testJob.Schedule( results.Length, 32 );

        handle.Complete();
        results.CopyTo( testValues );
        results.Dispose();

        Debug.LogWarning( "0 Index : " + testValues[0] );
    }
}

public struct JobStruct : IJobParallelFor
{
    public float deltaTime;
    public NativeArray<float> result;

    public void Execute(int index)
    {
        result[index] += math.sin( deltaTime );
    }
}

[BurstCompile(CompileSynchronously =true)]
public struct BurstStruct : IJobParallelFor {
    public float deltaTime;
    public NativeArray<float> result;

    public void Execute(int index) {
        result[index] += math.sin( deltaTime );
    }
}
