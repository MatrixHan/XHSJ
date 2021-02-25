using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
            music.loop = true;
            SetRandomMusic();
            music.Play();
        }
    }

    public AudioSource music;
    public AudioSource sound_effect;

    public AudioClip[] bg_music;
    public AudioClip[] ui_clip;

    public static void SetRandomMusic(MusicClipType min = MusicClipType.bg_1, MusicClipType max = MusicClipType.end) {
        SetMusic((MusicClipType)Random.Range((int)min, (int)max));
    }
    public static void SetMusic(MusicClipType bg) {
        instance.music.clip = instance.bg_music[(int)bg];
    }

    bool uimute = true;
    public static void PlayUIClip(UIClipType bg) {
        if (instance.uimute)
            return;
        instance.sound_effect.PlayOneShot(instance.ui_clip[(int)bg]);
    }
    public static void SetUIMute(bool isOn = false) {
        instance.uimute = isOn;
    }


    public enum MusicClipType {
        /// <summary>
        /// ����
        /// </summary>
        bg_1,
        /// <summary>
        /// �ؾ�
        /// </summary>
        bg_2,
        /// <summary>
        /// ����3
        /// </summary>
        bg_3,
        /// <summary>
        /// ����4
        /// </summary>
        bg_4,
        /// <summary>
        /// ս��5
        /// </summary>
        bg_5,
        /// <summary>
        /// ս��6
        /// </summary>
        bg_6,
        /// <summary>
        /// ս��7
        /// </summary>
        bg_7,
        /// <summary>
        /// Ұ��8
        /// </summary>
        bg_8,
        /// <summary>
        /// Ұ��9
        /// </summary>
        bg_9,


        end,
    }


    public enum UIClipType {
        /// <summary>
        /// �򿪱���
        /// </summary>
        bag_open,
        /// <summary>
        /// ���� ���� buff
        /// </summary>
        bottle,
        /// <summary>
        /// ����
        /// </summary>
        button_buy,
        /// <summary>
        /// ��ť�л�
        /// </summary>
        button_change,
        /// <summary>
        /// �رհ�ť
        /// </summary>
        button_close,
        /// <summary>
        /// ��ťͨ�õ��
        /// </summary>
        button_common,
        /// <summary>
        /// ��ť����
        /// </summary>
        button_in,
        /// <summary>
        /// Ǯ������
        /// </summary>
        coin,
        /// <summary>
        /// ���� �۵�һ��
        /// </summary>
        destiny,
        /// <summary>
        /// ����
        /// </summary>
        duanzao,
        /// <summary>
        /// װ��
        /// </summary>
        equip,
        /// <summary>
        /// �ϳ�
        /// </summary>
        hecheng,
        /// <summary>
        /// �µ���Ϣ
        /// </summary>
        new_message,
        /// <summary>
        /// ������
        /// </summary>
        stuff_bag,
        /// <summary>
        /// ����
        /// </summary>
        unlock = 14,
    }
}
