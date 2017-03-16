﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Artngame.PDM;

namespace Artngame.PDM {
	
[ExecuteInEditMode()]
[System.Serializable()]
public class SKinColoredMasked : MonoBehaviour {
	
	void OnEnable(){
		if(!Application.isPlaying){
			
			p11 = this.gameObject.GetComponent<ParticleSystem>();
		}
	}

	
	Vector3[] vertices ;
	Vector3[] normals ;
	
	public float Scale_factor=1f;
	Color32[] colorsA  ;
	
	public float Start_size=0.2f;
	
	Color[] Pcolors;
	
	
	ParticleSystem.Particle[] ParticleList;
	
	
	public Texture2D mask; 
	Color32[] colorsMASK;
	public int low_mask_thres=100;
	public bool size_by_mask=false;
	private int masked_particle_count;
	
	public GameObject emitter;
	
	[SerializeField,HideInInspector]
	public SkinnedMeshRenderer mesh ;
	
	[SerializeField,HideInInspector]
	public MeshFilter simple_mesh ;
	
	[SerializeField,HideInInspector]
	public Mesh animated_mesh;
	
	
	public bool Colored=true;
		public bool Custom_Color=false;
		public bool Lerp_Color=false;

		public Color End_color = Color.white;
	
	private bool got_colors=false;
	
	public bool face_emit=false;

	public bool follow_particle=false;

		//v1.4
		public bool Velocity_toward_normal=false;
		public Vector3 Normal_Velocity=new Vector3(0,0,0);

	public void Start () {
		
		if(simple_mesh!=null & 1==1){
			
			
			if(1==1){
				if(Application.isPlaying){
					vertices = simple_mesh.mesh.vertices;
				}
				else{
					vertices = simple_mesh.sharedMesh.vertices;
				}
				
				
				
				
				if(!face_emit){
					
						if(mask==null){
							if(p11.maxParticles!=(int)(vertices.Length/Every_other_vertex)){
								p11.maxParticles=(int)(vertices.Length/Every_other_vertex);
							}
							
							
							p11.Emit((int)(vertices.Length/Every_other_vertex));
						}
				}
				if(face_emit){
					
					if(!let_loose){
						p11.Emit(p11.maxParticles);
					}
					
				}
			}
			
		}
		
		if(mesh!=null){
			
			if(animated_mesh!=null){
				mesh.BakeMesh(animated_mesh);
				
				
				vertices = animated_mesh.vertices;
				{
					
					
					if(!face_emit){
						

								if(p11.maxParticles!=(int)(vertices.Length/Every_other_vertex)){
									p11.maxParticles=(int)(vertices.Length/Every_other_vertex);
								}
								
								p11.Emit((int)(vertices.Length/Every_other_vertex)); 

					}
					if(face_emit){
						
						p11.Emit(p11.maxParticles); 
					}
					
					
					
				}
			}
		}
		
		
		Particle_Num = particle_count;
		
		Registered_paint_positions = new List<Vector3>();
		Registered_paint_rotations = new List<Vector3>();
		
		noise = new PerlinPDM ();
		
		if(Application.isPlaying){

			if(!face_emit & mask==null){
				
				p11.Clear ();
				
			}
			
		}
		
		if(p11==null ){
			p11 = this.gameObject.GetComponent<ParticleSystem>();
		}
		
		if(p11 !=null){
			
			keep_max_particle_number = p11.maxParticles;
			
		}else{
			
			Debug.Log ("Please attach a gameobject with skinned mesh or meshfilter as emitter and attach script to a particle system");
		}


			//last_baked = 0;
		
	}
	
	private int keep_max_particle_number;
	
	public float Every_other_vertex=1;
	
	public int particle_count = 100;
	
	bool got_positions=false;

	Vector3[] positions;
	Vector2[] rand_offsets;
	int[] tile;
	
	public ParticleSystem p11;
	
	private List<Vector3> Registered_paint_positions; 
	private List<Vector3> Registered_paint_rotations; 
	
	public float Y_offset=0f;
	
	public bool fix_initial = false;
	private bool let_loose = false;
	public bool Let_loose = false;
	
	public float keep_in_position_factor =0.95f;
	public float keep_alive_factor =0.5f;
	
	private int place_start_pos;
	
	public bool extend_life=false;
	
	public bool Gravity_Mode=false;
	
	private int Particle_Num;
	
	private PerlinPDM  noise;

	public float Return_speed=0.005f;

	public bool follow_normals = false;

		public bool Transition=false;
	

		[HideInInspector]
		public float bake_interval = 0.5f;
		//private float last_baked;
		[HideInInspector]
		public bool no_bake=false;

		Mesh Shared_mesh;

	void LateUpdate () {
		
		if(emitter ==null | p11 == null){
						
			//Debug.Log ("Please attach a gameobject with skinned mesh or meshfilter as emitter and attach script to a particle system"); 
			
			return;
		}

			if(Registered_paint_positions==null){
				Registered_paint_positions = new List<Vector3>();
			}
			if(Registered_paint_rotations==null){
				Registered_paint_rotations = new List<Vector3>();
			}

		
		if(Every_other_vertex <0.05f){
			Every_other_vertex = 0.05f;
		}
				
		if(mesh ==null & Application.isPlaying){
			mesh = emitter.GetComponent<SkinnedMeshRenderer>();
		}
				
		if(animated_mesh==null ){animated_mesh = new Mesh();animated_mesh.hideFlags = HideFlags.HideAndDontSave;}
		
		if(animated_mesh==null & Application.isPlaying){
			
			if(animated_mesh==null){
				animated_mesh = new Mesh();
			}
			if(mesh!=null){
				mesh.BakeMesh(animated_mesh);
			}
				//Debug.Log ("check1");
			
		}
		
		if(simple_mesh==null & Application.isPlaying){
			simple_mesh = emitter.GetComponent<MeshFilter>();
		}


			if(Transition){
				simple_mesh = emitter.GetComponent<MeshFilter>();
				mesh = emitter.GetComponent<SkinnedMeshRenderer>();
				Transition=false;
				colorsA=null;
				colorsMASK =null;
			}




		////////////////// SKINNED //////////////

			Vector3 Global_pos_transform=emitter.transform.position;


		if(p11 != null){ 
			if(p11.startSize < Start_size & Application.isPlaying){
				p11.startSize = p11.startSize+(Start_size/3);
			}else{
				
			}
			
			//reset if changed parricle max number
			if(p11.maxParticles != keep_max_particle_number & mask==null){
				Start ();
				got_positions=false;
				positions=null;
				keep_max_particle_number=p11.maxParticles;
				Debug.Log ("adjusted");
			}
			
				if(follow_particle){
					Global_pos_transform = p11.transform.position;
				}else{

					Global_pos_transform = emitter.transform.position;
				}

		}
		
		if(Every_other_vertex<=0){
			Every_other_vertex=1;
		}

		if(Every_other_vertex<1 & mask!=null){
			Every_other_vertex=1;
		}
		
		///////////////// END SKINNED //////////////
							
			if(Particle_Num != particle_count){
				
				got_positions=false;
				Particle_Num = particle_count;
			}
			
			let_loose = Let_loose;
			if(!Application.isPlaying){ 
				
				//let_loose = false;
				
				if(noise ==null){
					noise = new PerlinPDM ();
				}
				
			}
			
			int tileCount=15;
			
			#region MESHES
			
			if(simple_mesh!=null){
				
				if(Application.isPlaying){
					vertices = simple_mesh.mesh.vertices;
					normals = simple_mesh.mesh.normals;
				}
				else{
					vertices = simple_mesh.sharedMesh.vertices;
					normals = simple_mesh.sharedMesh.normals;
				}
				
				
				if(!face_emit){

					if(mask==null){
						if(p11.maxParticles!=(int)(vertices.Length/Every_other_vertex)){
							p11.maxParticles=(int)(vertices.Length/Every_other_vertex);
						}
												
						p11.Emit((int)(vertices.Length/Every_other_vertex));
					}
					if(mask!=null){
						if(p11.maxParticles!=(int)masked_particle_count){
							p11.maxParticles=(int)masked_particle_count;
							Debug.Log ("ppp");
						}
						if(!extend_life | !Application.isPlaying){
							p11.Emit((int)masked_particle_count);
						}
					}
				}
				if(face_emit){
					
					if(mask==null){
					if(p11.particleCount<p11.maxParticles){

						

							p11.Emit(p11.maxParticles);

						
						}
					}
					if(mask!=null){
						if(p11.particleCount<masked_particle_count){
							

								if(!extend_life | !Application.isPlaying){
									p11.Emit(masked_particle_count);
								}

						}
					}
				}
				
			}
			
			if(mesh!=null){
				
				if(animated_mesh!=null){


					if(!no_bake | 1==0){
					
						mesh.BakeMesh(animated_mesh);

					
					}

					if((no_bake & vertices==null) | !no_bake){
						vertices = animated_mesh.vertices;
					}

					if((no_bake & normals==null) | !no_bake){
						normals = animated_mesh.normals;
					}

					if(no_bake & 1==1){

						if(Shared_mesh==null){

							Shared_mesh=mesh.sharedMesh;

						}

						Matrix4x4[] boneMatrix =new Matrix4x4[mesh.bones.Length];



						for(int i=0;i<boneMatrix.Length;i++){

							boneMatrix[i] = mesh.bones[i].localToWorldMatrix*Shared_mesh.bindposes[i];


							if(1==1){
							for(int j=0;j<Shared_mesh.vertexCount;j=j+(int)Every_other_vertex){

								BoneWeight weight = Shared_mesh.boneWeights[j];
								Matrix4x4 j0 = boneMatrix[weight.boneIndex0];
								Matrix4x4 j1 = boneMatrix[weight.boneIndex1];
								Matrix4x4 j2 = boneMatrix[weight.boneIndex2];
								Matrix4x4 j3 = boneMatrix[weight.boneIndex3];

								Matrix4x4 vertexes = new Matrix4x4();

								for(int k=0;k<16;k++){

									vertexes[k] = j0[k]*weight.weight0 + j1[k]*weight.weight1 + j2[k]*weight.weight2 + j3[k]*weight.weight3 ;

								}

								vertices[j] = vertexes.MultiplyPoint3x4(Shared_mesh.vertices[j]);
								normals[i] = vertexes.MultiplyVector(Shared_mesh.normals[i]);

							}

						}
						}

					}

					if(1==1)
					{
						
						if(!face_emit){
							
							//AA
							if(mask==null){
								Debug.Log (p11.maxParticles);
								if(p11.maxParticles!=(int)(vertices.Length/Every_other_vertex)){
									p11.maxParticles=(int)(vertices.Length/Every_other_vertex);
									Debug.Log (p11.maxParticles);
								}
								
								p11.Emit((int)(vertices.Length/Every_other_vertex)); 
							}
							if(mask!=null){

								if(p11.maxParticles!=(int)(masked_particle_count)){
									p11.maxParticles=(int)(masked_particle_count);
									//Debug.Log ("after = "+p11.maxParticles);
								}
								if(!extend_life | !Application.isPlaying){
									p11.Emit((int)(masked_particle_count));
								}
							}
							
						}
						if(face_emit){
							
							//p11.Emit(p11.maxParticles); //v1.2.2

							//v1.3
							if(mask==null){
								if(p11.particleCount<p11.maxParticles){
									

									
									p11.Emit(p11.maxParticles);
									

								}
							}
							if(mask!=null){
								if(p11.particleCount<masked_particle_count){
									

									if(!extend_life | !Application.isPlaying){
										p11.Emit(masked_particle_count);
									}

								}
							}

						}
						
					}
				}
			}
			
			
			if(vertices!=null){
				int DIVIDED_VERTEXES = (int)(vertices.Length/Every_other_vertex);
				
				if(face_emit){
					DIVIDED_VERTEXES = p11.maxParticles;
				}
								
				if(  p11.particleCount >= (DIVIDED_VERTEXES) | mask!=null ){ 
					
					if(simple_mesh!=null){
						
						if(Application.isPlaying){
							vertices = simple_mesh.mesh.vertices;
						}else{vertices = simple_mesh.sharedMesh.vertices;}
						
						int ParticlesNeeded = vertices.Length;
							int count_masked_particles=0;
						
						if(p11.particleCount < ParticlesNeeded){
						}
						
						int Count_uvs =0;
						
						if(Application.isPlaying){
							
							Count_uvs = simple_mesh.mesh.uv2.Length;
						}else{
							
							Count_uvs = simple_mesh.sharedMesh.uv2.Length;
						}

						if(Count_uvs==0){

							if(Application.isPlaying){
								
								Count_uvs = simple_mesh.mesh.uv.Length;
							}else{
								
								Count_uvs = simple_mesh.sharedMesh.uv.Length;
							}

						}
						
						Vector2[] uvs    = new Vector2[Count_uvs];
						
						if(Application.isPlaying){
							
							uvs = simple_mesh.mesh.uv2;
						}else{
							
							uvs = simple_mesh.sharedMesh.uv2; 
						}

						if(uvs.Length==0){

							if(Application.isPlaying){
								
								uvs = simple_mesh.mesh.uv;
							}else{
								
								uvs = simple_mesh.sharedMesh.uv; 
							}

						}

							if(Colored){
								if(colorsA==null){
									colorsA = new Color32[ uvs.Length ]; 
								}
							}

							
							if(colorsMASK==null){
								colorsMASK = new Color32[ uvs.Length ]; 
							}

						Vector4 offset1 = new Vector4(0,0,0,0);
						
					
						
						Texture2D pixels = null;

						if(Application.isPlaying){
							if(simple_mesh.gameObject.GetComponent<Renderer>().material !=null){
								if(simple_mesh.gameObject.GetComponent<Renderer>().material.HasProperty("_MainTex")){
									//Debug.Log ("it has ?");
									if(simple_mesh.gameObject.GetComponent<Renderer>().material.mainTexture !=null){
										pixels =  simple_mesh.gameObject.GetComponent<Renderer>().material.mainTexture as Texture2D;
									}
								}
							}

						}else{

							if(simple_mesh.gameObject.GetComponent<Renderer>().sharedMaterial.HasProperty("_MainTex")){
								if(simple_mesh.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture !=null){
									pixels =  simple_mesh.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
								}
							}
						}

						if(Application.isPlaying){
							if(pixels!=null ){
								offset1 = simple_mesh.gameObject.GetComponent<Renderer>().material.mainTextureOffset;
							}
						}
						else{
							if(pixels!=null ){
								offset1 = simple_mesh.gameObject.GetComponent<Renderer>().sharedMaterial.mainTextureOffset;
							}
						}


						int uvl = uvs.Length;
						
						if(this.transform.parent != null){
							
							//if(this.transform.parent.renderer.sharedMaterial.mainTexture!=null ){
							if(pixels!=null ){
								if(this.transform.parent.GetComponent<Renderer>().sharedMaterial.mainTexture.filterMode == FilterMode.Bilinear){
									for ( int j=0; j<uvl; j=j+(int)Every_other_vertex) {
										
										Vector2 uv = uvs[ j ];
										

										if(Colored & pixels !=null){
											colorsA[ j ] = pixels.GetPixelBilinear( ( uv.x)+offset1.x , ( uv.y)+offset1.y  );
										}
										
											if(mask!=null){

												colorsMASK[ j ] = mask.GetPixelBilinear( ( uv.x) , ( uv.y)  );

												if(colorsMASK[ j ].a <low_mask_thres){

													if(Colored){
														colorsA[ j ].a = 0;
													}
													
												}else{
													count_masked_particles++;
												}
																								
											}
										
									}
								}else{
									if(Colored & pixels !=null){
										colorsA = pixels.GetPixels32();
									}
										
								}
							}
							masked_particle_count = count_masked_particles;
							
						}else{
							Debug.Log ("Please attach the particle to the emitter mesh object");
						}
						
							#region MASK - SIMPLE MESH

							if(p11 != null & mask!=null){ 

								if(mask!=null){
									
									int keep_at_one = (int)Every_other_vertex;
									if(keep_at_one <1){ keep_at_one=1; }

									keep_at_one=1;

									if(p11.maxParticles!=(int)(count_masked_particles/keep_at_one)){

										if(low_mask_thres>255){
											p11.Clear();
										}

										masked_particle_count = (int)(count_masked_particles/keep_at_one);

										p11.maxParticles=(int)(count_masked_particles/keep_at_one);
										p11.Emit((int)(count_masked_particles/keep_at_one)); 

										

									}
									

								}
																
								ParticleList = new ParticleSystem.Particle[p11.particleCount];
								p11.GetParticles(ParticleList);
								
								//int count_vertices=0;
								
								int count_particles=0;
								
								for (int i=0; i < vertices.Length;i=i+(int)Every_other_vertex)
								{	
										
									if(colorsMASK[ i ].a <low_mask_thres){}else{
											
											if(Colored){
												ParticleList[count_particles].startColor = colorsA[i];

												if(Custom_Color){
													if(Lerp_Color){
												ParticleList[count_particles].startColor = Color.Lerp (ParticleList[count_particles].startColor,End_color,0.5f);
													}else{
												ParticleList[count_particles].startColor = End_color;
													}
												}
											}
											
									ParticleList[count_particles].startSize =Start_size;
											Random.seed=i;
									ParticleList[count_particles].startSize = Random.Range(Start_size/1.5f,0.010f);
											
											if(size_by_mask){
										ParticleList[count_particles].startSize = 0.4f*(colorsMASK[i].r/250);
											}
											
											///////////////// PARTICLES /////////////////
											
											if(extend_life){
												
										 				tileCount=15;
														ParticleList[count_particles].remainingLifetime = tileCount + 1 - 11;
													
											}
											
											if(ParticleList[count_particles].remainingLifetime < ParticleList[count_particles].startLifetime*keep_alive_factor){
												ParticleList[count_particles].startLifetime = tileCount;
											}
											

									if(let_loose){
										
										//if(positions!=null){ 
											//if(positions!=null & count_particles<positions.Length){
										if(count_particles < ParticleList.Length){
												
												if(!extend_life & ParticleList[count_particles].remainingLifetime > ParticleList[count_particles].startLifetime*keep_in_position_factor){
													if(!face_emit | 1==1){
														ParticleList[count_particles].position = emitter.transform.rotation*new Vector3(vertices[i].x*emitter.transform.localScale.x,vertices[i].y*emitter.transform.localScale.y,vertices[i].z*emitter.transform.localScale.z)*Scale_factor + new Vector3(0f,0f,0f)+ Global_pos_transform;
																											
													}

												}

											//v1.4
											if(ParticleList[count_particles].remainingLifetime > ParticleList[count_particles].startLifetime*keep_in_position_factor){
												if(Velocity_toward_normal){
													ParticleList[count_particles].velocity = emitter.transform.rotation*new Vector3(normals[i].normalized.x*Normal_Velocity.x,normals[i].normalized.y*Normal_Velocity.y,normals[i].normalized.z*Normal_Velocity.z); 
												}
											}
												
												//Gravity
												if(let_loose & Gravity_Mode){

													ParticleList[count_particles].position = Vector3.Slerp(ParticleList[count_particles].position,emitter.transform.rotation*new Vector3(vertices[i].x*emitter.transform.localScale.x,vertices[i].y*emitter.transform.localScale.y,vertices[i].z*emitter.transform.localScale.z)*Scale_factor + new Vector3(0f,0f,0f)+ Global_pos_transform,Return_speed);

													ParticleList[count_particles].velocity= Vector3.Slerp(ParticleList[count_particles].velocity,Vector3.zero,0.05f);
												}
											}
										//}
										
									}


											if(!let_loose){
										ParticleList[count_particles].position = emitter.transform.rotation*(new Vector3(vertices[i].x*emitter.transform.localScale.x,vertices[i].y*emitter.transform.localScale.y,vertices[i].z*emitter.transform.localScale.z)*Scale_factor+new Vector3(0f,0f,0f))+ Global_pos_transform;
											

											if(!follow_normals){

												ParticleList[count_particles].rotation = 90;
												
												ParticleList[count_particles].axisOfRotation = normals[i];
											}

											if(follow_normals){
											
												ParticleList[count_particles].rotation = 90;

												float FIX_for_Z = 90;

												Quaternion FIX_ROT = emitter.transform.rotation;
												Vector3 FIXED_NORMAL = FIX_ROT*normals[i];

												if(FIXED_NORMAL.z >=0 & FIXED_NORMAL.x >=0){
													FIX_for_Z = FIX_for_Z - Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) ); //- Mathf.Abs((vertices[i].z-normals[i].z));
												}
												if(FIXED_NORMAL.z <0 & FIXED_NORMAL.x >=0){
													FIX_for_Z = FIX_for_Z + Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) ); //- Mathf.Abs((vertices[i].z-normals[i].z));
												}

												if(FIXED_NORMAL.z >=0 & FIXED_NORMAL.x <0){
													FIX_for_Z = FIX_for_Z + Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) )+180; //- Mathf.Abs((vertices[i].z-normals[i].z));
												}
												if(FIXED_NORMAL.z <0 & FIXED_NORMAL.x <0){
													FIX_for_Z = FIX_for_Z - Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) )+180; //- Mathf.Abs((vertices[i].z-normals[i].z));
												}
											
												float DOT = Vector3.Dot(new Vector3(0.0000001f,0.0000001f,1),FIXED_NORMAL);
												float ACOS = 0f; 
												if(DOT >1 | DOT <-1){
												}else{
													ACOS= Mathf.Acos(DOT);
												}

												ParticleList[count_particles].rotation = ACOS+FIX_for_Z;
												ParticleList[count_particles].axisOfRotation =  Vector3.Cross( new Vector3(0.00000001f,0.00000001f,1), FIXED_NORMAL);
																							
											}

											bool assign_normal_velocity=false;
											if(assign_normal_velocity){
												ParticleList[count_particles].velocity = 0.1f*normals[i];
											}
										}
											///////////////// END PARTICLES
											
											if(count_particles<ParticleList.Length-1){
												count_particles++;
											}
											else
											{
												count_particles=0;
											}
											
										}
									
								}
																						
								p11.SetParticles(ParticleList,p11.particleCount);
								
							}//end if p11 !=null & mask!=null

							#endregion
												
						if(p11 != null & mask==null){ 
														
							ParticleList = new ParticleSystem.Particle[p11.particleCount];
							p11.GetParticles(ParticleList);
							
							if(p11.particleCount >=(DIVIDED_VERTEXES)){
								
								int count_vertices=0;
								for (int i=0; i < ParticleList.Length;i++)
								{
									if(extend_life){
										if(tile!=null){
											if(i<tile.Length){
												ParticleList[i].remainingLifetime = tileCount + 1 - tile[i];
											}
										}
									}
									
									if(ParticleList[i].remainingLifetime < ParticleList[i].startLifetime*keep_alive_factor){
										ParticleList[i].startLifetime = tileCount;
									}
									
									if(!let_loose){
										
										if(!face_emit){
											ParticleList[i].position = emitter.transform.rotation*new Vector3(vertices[count_vertices].x*emitter.transform.localScale.x,vertices[count_vertices].y*emitter.transform.localScale.y,vertices[count_vertices].z*emitter.transform.localScale.z)*Scale_factor + new Vector3(0f,0f,0f)+ Global_pos_transform;
										}else{
											
											if(positions!=null ){ 
												if(positions!=null & i<positions.Length){
													ParticleList[i].position = positions[i];
												}}
										}
										
									}
									if(let_loose){									


										if(i < ParticleList.Length){
											
											if(!extend_life & ParticleList[i].remainingLifetime > ParticleList[i].startLifetime*keep_in_position_factor){
												if(!face_emit | 1==1){
													ParticleList[i].position = emitter.transform.rotation*new Vector3(vertices[count_vertices].x*emitter.transform.localScale.x,vertices[count_vertices].y*emitter.transform.localScale.y,vertices[count_vertices].z*emitter.transform.localScale.z)*Scale_factor + new Vector3(0f,0f,0f)+ Global_pos_transform;
												}
												
											}

											//v1.4
											if(ParticleList[i].remainingLifetime > ParticleList[i].startLifetime*keep_in_position_factor){
												if(Velocity_toward_normal){
													ParticleList[i].velocity = emitter.transform.rotation*new Vector3(normals[count_vertices].normalized.x*Normal_Velocity.x,normals[count_vertices].normalized.y*Normal_Velocity.y,normals[count_vertices].normalized.z*Normal_Velocity.z); 
												}
											}
											
											//Gravity
											if(let_loose & Gravity_Mode){
												
												ParticleList[i].position = Vector3.Slerp(ParticleList[i].position,emitter.transform.rotation*new Vector3(vertices[count_vertices].x*emitter.transform.localScale.x,vertices[count_vertices].y*emitter.transform.localScale.y,vertices[count_vertices].z*emitter.transform.localScale.z)*Scale_factor + new Vector3(0f,0f,0f)+ Global_pos_transform,Return_speed);
												
												ParticleList[i].velocity= Vector3.Slerp(ParticleList[i].velocity,Vector3.zero,0.05f);
											}
										}
										
									}
									
									if(Colored){
										ParticleList[i].startColor = colorsA[count_vertices];

										if(Custom_Color){
											if(Lerp_Color){
												ParticleList[i].startColor = Color.Lerp (ParticleList[i].startColor,End_color,0.5f);
											}else{
												ParticleList[i].startColor = End_color;
											}
										}
									}
									
									count_vertices=count_vertices+1;
									
									if(count_vertices>vertices.Length-1){
										count_vertices=0;
									}
								}
								
								p11.SetParticles(ParticleList,p11.particleCount);
							}
						}
						
					} // END SIMPLE MESH
					


					if(mesh!=null){


												
						Vector2[] uvs    =  animated_mesh.uv2;

						if(uvs.Length==0){
							uvs    =  animated_mesh.uv;
						}



						if(uvs.Length<vertices.Length){
							//DO NOTHING
						}
						else
						{
						
							if(Colored){
								if(colorsA==null){
									colorsA = new Color32[ uvs.Length ]; 
								}
							}
													
						if(colorsMASK==null){
						colorsMASK = new Color32[ uvs.Length ]; 
						}

						Texture2D pixels = mesh.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;

						

						int uvl = uvs.Length;
						int count_masked_particles=0;
						if(!got_colors){

						for ( int j=0; j<uvl; j=j+(int)Every_other_vertex) {

							Vector2 uv = uvs[ j ];
							
									if(Colored & pixels != null){
										colorsA[ j ] = pixels.GetPixelBilinear( ( uv.x) , ( uv.y)  );
									}
							
							if(mask!=null){
																
								colorsMASK[ j ] = mask.GetPixelBilinear( ( uv.x) , ( uv.y)  );
								
								if(colorsMASK[ j ].a <low_mask_thres){
									
											if(Colored){
												colorsA[ j ].a = 0;
											}
									
								}else{
									count_masked_particles++;
								}
																
							}
							
							}
						}
						masked_particle_count = count_masked_particles;
						
						if(p11 != null & mask!=null){ 
							
							if(mask!=null){
							
								int keep_at_one = (int)Every_other_vertex;
								if(keep_at_one <1){ keep_at_one=1; }

									keep_at_one=1;

								if(p11.maxParticles!=(int)(count_masked_particles/keep_at_one)){
									
										if(low_mask_thres>255){
											p11.Clear();
										}

										//Debug.Log (p11.maxParticles);
										masked_particle_count = (int)(count_masked_particles/keep_at_one);

									p11.maxParticles=(int)(count_masked_particles/keep_at_one);
										p11.Emit((int)(count_masked_particles/keep_at_one));
								}
								
							}
														
							ParticleList = new ParticleSystem.Particle[p11.particleCount];
							p11.GetParticles(ParticleList);


							
							//int count_vertices=0;
							
							int count_particles=0;
							
							if(ParticleList.Length>0){
							for (int i=0; i < vertices.Length;i=i+(int)Every_other_vertex)
							{								
								if(1==1){
									
									
									if(colorsMASK[ i ].a <low_mask_thres){}else{
										
										if(Colored){
													ParticleList[count_particles].startColor = colorsA[i];

													if(Custom_Color){
														if(Lerp_Color){
															ParticleList[count_particles].startColor = Color.Lerp (ParticleList[count_particles].startColor,End_color,0.5f);
														}else{
															ParticleList[count_particles].startColor = End_color;
														}
													}

										}
										
												ParticleList[count_particles].startSize =Start_size;
										Random.seed=i;
												ParticleList[count_particles].startSize = Random.Range(Start_size/1.5f,0.010f);
										
										if(size_by_mask){
											
													ParticleList[count_particles].startSize = 0.4f*(colorsMASK[i].r/250);
										}
										
										///////////////// PARTICLES /////////////////
										
										if(extend_life){
										
													tileCount=15;
													ParticleList[count_particles].remainingLifetime = tileCount + 1 - 11;
												
										}
										
										if(ParticleList[count_particles].remainingLifetime < ParticleList[count_particles].startLifetime*keep_alive_factor){
											ParticleList[count_particles].startLifetime = tileCount;
										}
										
											if(let_loose){
												if(!extend_life & ParticleList[count_particles].remainingLifetime > (ParticleList[count_particles].startLifetime*keep_in_position_factor)){
													
														ParticleList[count_particles].position = emitter.transform.rotation*(new Vector3(vertices[i].x*emitter.transform.localScale.x,vertices[i].y*emitter.transform.localScale.y,vertices[i].z*emitter.transform.localScale.z)*Scale_factor+new Vector3(0f,0f,0f))+ Global_pos_transform;

												}
												
													//v1.4
													if(ParticleList[count_particles].remainingLifetime > ParticleList[count_particles].startLifetime*keep_in_position_factor){
														if(Velocity_toward_normal){
															ParticleList[count_particles].velocity = emitter.transform.rotation*new Vector3(normals[i].normalized.x*Normal_Velocity.x,normals[i].normalized.y*Normal_Velocity.y,normals[i].normalized.z*Normal_Velocity.z); 
														}
													}

												//Gravity
												if(let_loose & Gravity_Mode){
													
														ParticleList[count_particles].position = Vector3.Slerp(ParticleList[count_particles].position, (emitter.transform.rotation*(new Vector3(vertices[i].x*emitter.transform.localScale.x,vertices[i].y*emitter.transform.localScale.y,vertices[i].z*emitter.transform.localScale.z)*Scale_factor+new Vector3(0f,0f,0f))+ Global_pos_transform),Return_speed);
													
													ParticleList[count_particles].velocity= Vector3.Slerp(ParticleList[count_particles].velocity,Vector3.zero,0.05f);
												}
												
											}

										
										if(!let_loose){

													ParticleList[count_particles].position = emitter.transform.rotation*(new Vector3(vertices[i].x*emitter.transform.localScale.x,vertices[i].y*emitter.transform.localScale.y,vertices[i].z*emitter.transform.localScale.z)*Scale_factor+new Vector3(0f,0f,0f))+ Global_pos_transform;
											
																														
												if(!follow_normals){
													ParticleList[count_particles].rotation = 100;
											
													ParticleList[count_particles].axisOfRotation = normals[i];
												}

												if(follow_normals){

													ParticleList[count_particles].rotation = 90;
													
													float FIX_for_Z = 90;
													
													Quaternion FIX_ROT = emitter.transform.rotation;
													Vector3 FIXED_NORMAL = FIX_ROT*normals[i];
													
													if(FIXED_NORMAL.z >=0 & FIXED_NORMAL.x >=0){
														FIX_for_Z = FIX_for_Z - Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) ); //- Mathf.Abs((vertices[i].z-normals[i].z));
													}
													if(FIXED_NORMAL.z <0 & FIXED_NORMAL.x >=0){
														FIX_for_Z = FIX_for_Z + Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) ); //- Mathf.Abs((vertices[i].z-normals[i].z));
													}
													
													if(FIXED_NORMAL.z >=0 & FIXED_NORMAL.x <0){
														FIX_for_Z = FIX_for_Z + Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) )+180; //- Mathf.Abs((vertices[i].z-normals[i].z));
													}
													if(FIXED_NORMAL.z <0 & FIXED_NORMAL.x <0){
														FIX_for_Z = FIX_for_Z - Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) )+180; //- Mathf.Abs((vertices[i].z-normals[i].z));
													}

													float DOT = Vector3.Dot(new Vector3(0.0000001f,0.0000001f,1),FIXED_NORMAL);
													float ACOS = 0f; 
													if(DOT >1 | DOT <-1){
													}else{
														ACOS= Mathf.Acos(DOT);
													}

													ParticleList[count_particles].rotation = ACOS+FIX_for_Z;
													ParticleList[count_particles].axisOfRotation =  Vector3.Cross( new Vector3(0.00000001f,0.00000001f,1), FIXED_NORMAL);

												}
										}												
										///////////////// END PARTICLES
										
										if(count_particles<ParticleList.Length-1){
											count_particles++;
										}
										else
										{
											count_particles=0;
										}
										
									}
									
								}
								
								}
							
							
								p11.SetParticles(ParticleList,p11.particleCount);
							}
						}//end if p11 !=null & mask!=null

						if(p11 != null & mask==null){ 
							
							if(p11.particleCount >=(DIVIDED_VERTEXES)){
								ParticleList = new ParticleSystem.Particle[p11.particleCount];
								p11.GetParticles(ParticleList);
								
								int count_vertices=0;
								
								for (int i=0; i < ParticleList.Length;i=i+1)
								{									
									if(extend_life){
										if(tile!=null){
											if(i<tile.Length){
												ParticleList[i].remainingLifetime = tileCount + 1 - tile[i];
											}
										}
									}
									
									if(ParticleList[i].remainingLifetime < ParticleList[i].startLifetime*keep_alive_factor){
										ParticleList[i].startLifetime = tileCount;
									}
									
									if(!let_loose){
										
											ParticleList[i].position = emitter.transform.rotation*(new Vector3(vertices[count_vertices].x*emitter.transform.localScale.x,vertices[count_vertices].y*emitter.transform.localScale.y,vertices[count_vertices].z*emitter.transform.localScale.z)*Scale_factor+new Vector3(0f,0f,0f))+ Global_pos_transform;
										
									}
									
									if(let_loose){
										if(!extend_life & ParticleList[i].remainingLifetime > (ParticleList[i].startLifetime*keep_in_position_factor)){
											
												ParticleList[i].position = emitter.transform.rotation*(new Vector3(vertices[count_vertices].x*emitter.transform.localScale.x,vertices[count_vertices].y*emitter.transform.localScale.y,vertices[count_vertices].z*emitter.transform.localScale.z)*Scale_factor+new Vector3(0f,0f,0f))+ Global_pos_transform;
											
										}
										
											//v1.4
											if(ParticleList[i].remainingLifetime > ParticleList[i].startLifetime*keep_in_position_factor){
												if(Velocity_toward_normal){
													ParticleList[i].velocity = emitter.transform.rotation*new Vector3(normals[count_vertices].normalized.x*Normal_Velocity.x,normals[count_vertices].normalized.y*Normal_Velocity.y,normals[count_vertices].normalized.z*Normal_Velocity.z); 
												}
											}

										//Gravity
										if(let_loose & Gravity_Mode){
											
												ParticleList[i].position = Vector3.Slerp(ParticleList[i].position, (emitter.transform.rotation*(new Vector3(vertices[count_vertices].x*emitter.transform.localScale.x,vertices[count_vertices].y*emitter.transform.localScale.y,vertices[count_vertices].z*emitter.transform.localScale.z)*Scale_factor+new Vector3(0f,0f,0f))+ Global_pos_transform),Return_speed);
											
											ParticleList[i].velocity= Vector3.Slerp(ParticleList[i].velocity,Vector3.zero,0.05f);
										}
										
									}


									if(colorsMASK[ count_vertices ].a <low_mask_thres){}else{

										if(Colored){
												ParticleList[i].startColor = colorsA[count_vertices];

												if(Custom_Color){
													if(Lerp_Color){
														ParticleList[i].startColor = Color.Lerp (ParticleList[i].startColor,End_color,0.5f);
													}else{
														ParticleList[i].startColor = End_color;
													}
												}
										}
										
										ParticleList[i].startSize =Start_size;
										
										if(size_by_mask){
											
												ParticleList[i].startSize = 0.4f*(colorsMASK[count_vertices].r/250);
										}
										
									}
									
								
									count_vertices=count_vertices+1;
									
									if(count_vertices>vertices.Length-1){
										count_vertices=0;
									}
									
								}
								p11.SetParticles(ParticleList,p11.particleCount);
								
							}//end if(p11.particleCount >=(DIVIDED_VERTEXES)){
							
						}//end if p11 !=null
						
					}//end UVs check
					}//end SKINNED MESH
					
				}//end if(  p11.particleCount >= (DIVIDED_VERTEXES)){ 
				
			}//end if vertices != null check
			
			#endregion
			if(ParticleList	!=null){	
				
				if(!got_positions | 1==0){
					positions = new Vector3[p11.particleCount];
					tile = new int[p11.particleCount];
					got_positions = true;
					
					for(int i=0;i<ParticleList.Length;i++){
						
						positions[i] = ParticleList[i].position;
						tile[i] = Random.Range(0,15);
					}
					
				}
				
				// PROJECTION
								
				if(!fix_initial){
					Registered_paint_positions.Clear();
					Registered_paint_rotations.Clear();
				}
				
				if(Registered_paint_positions!=null){
					
					for(int i=0;i<ParticleList.Length;i++){
						
						Registered_paint_positions.Add(ParticleList[i].position);
						Registered_paint_rotations.Add(Vector3.zero);
					}
					
				}
				
			}
			
		
	}//end update
}
	
}


